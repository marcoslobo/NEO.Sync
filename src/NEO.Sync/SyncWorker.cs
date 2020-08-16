using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Neo;
using Neo.SmartContract;
using NEO.Api.Data;
using NEO.Api.Models;
using NeoModules.RPC.DTOs;
using NeoModules.RPC.Services.Asset;
using NeoModules.RPC.Services.Block;
using NeoModules.RPC.Services.Transactions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NEO.Api.Worker
{
    public class SyncWorker : BackgroundService
    {
        private readonly ILogger<SyncWorker> _logger;
        private readonly NeoContext context;
        private readonly NeoGetBlockCount getBlockCount;
        private readonly NeoGetBlock getBlock;
        private readonly NeoGetRawTransaction getTransaction;
        private readonly NeoGetAssetState getAsset;

        public SyncWorker(ILogger<SyncWorker> logger, IConfiguration iconfiguration)
        {
            _logger = logger;
            context = new NeoContext(iconfiguration);

            var settings = new Settings();
            var client = ClientFactory.GetClient(settings);
            getBlockCount = new NeoGetBlockCount(client);
            getBlock = new NeoGetBlock(client);
            getTransaction = new NeoGetRawTransaction(client);
            getAsset = new NeoGetAssetState(client);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Let's Sync NEO Blockchain!");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Worker starting at: {time}", DateTimeOffset.Now);

                    await SyncTask();

                    await Task.Delay(1000, stoppingToken);

                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "Error at: {time}", DateTimeOffset.Now);                    
                }
            }
        }

        private async Task SyncTask()
        {
            long iCount = 0;

            var lastBlock = context.Blocks.OrderByDescending(a => a.Index).FirstOrDefault();
            if (lastBlock != null)
                iCount = lastBlock.Index + 1;

            var blockNumber = await getBlockCount.SendRequestAsync();

            while (iCount != blockNumber)
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        //var block = await getBlock.SendRequestAsync(5908400);
                        var block = await getBlock.SendRequestAsync((int)iCount);

                        //TODO: criar extensão para remover os replace 0x

                        var blocks = new Blocks()
                        {
                            Hash = UInt256.Parse(block.Hash).ToString().Remove0x(),
                            MerkleRoot = UInt256.Parse(block.Merkleroot).ToString().Remove0x(),
                            Index = block.Index,
                            NextConsensus = block.NextConsensus.ToInteropMethodHash().ToString().Remove0x(),
                            Nonce = Convert.ToUInt64(block.Nonce, 16).ToString().Remove0x(),
                            Script = JsonConvert.SerializeObject(block.Script),
                            Time = new DateTime(block.Time),
                            Size = block.Size
                        };

                        context.Add(blocks);
                        await context.SaveChangesAsync();


                        //Tratar transações\\
                        foreach (var transactionDto in block.Transactions)
                        {
                            var transactionId = await SaveTransaction(transactionDto, blocks);

                            if (transactionDto.Type.ToLower() == "contracttransaction")
                            {
                                await SaveTransfer(transactionDto, block, transactionId);
                            }
                        }

                        await transaction.CommitAsync();

                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogInformation("Worker error at: {time} - error: {errorMessage}", DateTimeOffset.Now, ex.Message);
                    }
                }
                iCount += 1;
            }
        }

        private async Task<Assets> AssetGetOrAdd(string hash)
        {
            try
            {
                var asset = context.Assets.FirstOrDefault(a => a.Hash == hash.Remove0x());

                if (asset == null)
                {
                    var assetToAdd = await getAsset.SendRequestAsync(hash);
                    if (assetToAdd == null)
                        throw new Exception($"Asset {hash} not found on blockchain!");

                    var assetToAddLang = assetToAdd.Name.FirstOrDefault(a => a.Lang.ToLower() == "en");

                    var adminWallet = await WalletGetOrAdd(assetToAdd.Admin.Remove0x());
                    var ownerWallet = await WalletGetOrAdd(assetToAdd.Owner.Remove0x());


                    asset = new Assets()
                    {
                        Amount = decimal.Parse(assetToAdd.Amount),
                        Hash = hash.Remove0x(),
                        Name = assetToAddLang.AssetName,
                        Symbol = assetToAddLang.AssetName,
                        Type = assetToAdd.Type,
                        Precision = assetToAdd.Precision,
                        AdminWallet = adminWallet,
                        AdminWalletId = adminWallet.Id,
                        OwnerWallet = ownerWallet,
                        OwnerWalletId = ownerWallet.Id
                    };

                    await context.AddAsync<Assets>(asset);
                    await context.SaveChangesAsync();
                    return asset;


                }
                else return asset;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private async Task SaveTransfer(BlockTx transaction, Block block, long transactionId)
        {
            try
            {
                if (transaction.Vin.Any() && transaction.Vout.Any())
                {
                    foreach (var vOut in transaction.Vout)
                    {
                        var transactionsIn = new List<Transaction>();
                        foreach (var vin in transaction.Vin.Where(a => a.Vout == vOut.N))
                        {
                            var transactionVin = await getTransaction.SendRequestAsync(transaction.Vin.FirstOrDefault().TransactionId);
                            transactionsIn.Add(transactionVin);
                        }

                        var transactionFrom = transactionsIn.SelectMany(a => a.Vout.Where(b => b.Value == vOut.Value && b.Asset == vOut.Asset));

                        if (transactionFrom != null && transactionFrom.Any())
                        {
                            var addressFrom = transactionFrom.FirstOrDefault().Address;
                            var transfer = new Transfers();

                            var walletFrom = await WalletGetOrAdd(addressFrom);
                            var walletTo = await WalletGetOrAdd(vOut.Address);

                            transfer.AddressFrom = walletFrom;
                            transfer.AddressFromId = walletFrom.Id;

                            transfer.AddressTo = walletTo;
                            transfer.AddressToId = walletTo.Id;

                            transfer.Amount = decimal.Parse(vOut.Value);
                            transfer.BlockIndex = block.Index;
                            transfer.InsertedAt = new DateTime(block.Time);
                            transfer.TransactionId = transactionId;

                            var asset = await AssetGetOrAdd(vOut.Asset);
                            transfer.Asset = asset ?? throw new Exception($"Asset {vOut.Asset} not found!");
                            transfer.AssetId = asset.Id;

                            context.Add(transfer);
                            await context.SaveChangesAsync();

                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private async Task<Wallet> WalletGetOrAdd(string address)
        {
            var wallet = context.Wallets.FirstOrDefault(a => a.Address == address);
            if (wallet == null)
            {
                wallet = new Wallet() { Address = address };
                context.Wallets.Add(wallet);
                await context.SaveChangesAsync();
                return wallet;
            }
            else return wallet;
        }

        private async Task<long> SaveTransaction(BlockTx transaction, Blocks block)
        {
            try
            {


                var tran = new Transactions();
                tran.Hash = transaction.Txid.Remove0x();
                tran.Size = transaction.Size;

                tran.BindBlock(block);
                tran.NetFee = decimal.Parse(transaction.NetFee);
                tran.SysFee = decimal.Parse(transaction.SysFee);
                tran.Type = transaction.Type;
                tran.Version = transaction.Version;


                context.Add(tran);
                await context.SaveChangesAsync();

                return tran.Id;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
