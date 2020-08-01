using NEO.Api.Models;
using NEO.Api.Worker.Extensions;
using EvoLua.Blockchain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace NEO.Api.Data
{
    public partial class EvoLuaContext : DbContext
    {
        private readonly IConfiguration configuration;

        public EvoLuaContext(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public EvoLuaContext(DbContextOptions<EvoLuaContext> options)
            : base(options)
        {
        }

        //public virtual DbSet<AddressBalances> AddressBalances { get; set; }
        //public virtual DbSet<AddressBalancesQueue> AddressBalancesQueue { get; set; }
        //public virtual DbSet<AddressHistories> AddressHistories { get; set; }
        //public virtual DbSet<AddressTransactionBalances> AddressTransactionBalances { get; set; }
        //public virtual DbSet<AddressTransactionBalancesQueue> AddressTransactionBalancesQueue { get; set; }
        //public virtual DbSet<AddressTransactions> AddressTransactions { get; set; }
        //public virtual DbSet<Addresses> Addresses { get; set; }
        //public virtual DbSet<AddressesQueue> AddressesQueue { get; set; }
        //public virtual DbSet<Assets> Assets { get; set; }
        //public virtual DbSet<Blocks> Blocks { get; set; }
        //public virtual DbSet<BlocksMeta> BlocksMeta { get; set; }
        //public virtual DbSet<BlocksQueue> BlocksQueue { get; set; }
        //public virtual DbSet<Claims> Claims { get; set; }
        //public virtual DbSet<Counters> Counters { get; set; }
        //public virtual DbSet<CountersCached> CountersCached { get; set; }
        //public virtual DbSet<CountersQueue> CountersQueue { get; set; }        
        //public virtual DbSet<TransactionAssets> TransactionAssets { get; set; }
        //public virtual DbSet<Transactions> Transactions { get; set; }
        //public virtual DbSet<Transfers> Transfers { get; set; }
        //public virtual DbSet<Vins> Vins { get; set; }
        //public virtual DbSet<Vouts> Vouts { get; set; }
        //public virtual DbSet<VoutsQueue> VoutsQueue { get; set; }

        public virtual DbSet<Transactions> Transactions { get; set; }
        public virtual DbSet<Wallet> Wallets { get; set; }

        public virtual DbSet<Assets> Assets { get; set; }
        public virtual DbSet<Blocks> Blocks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Database=evolua;Username=postgres;Password=postgres");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.NamesToSnakeCase();

            modelBuilder.Entity<Wallet>(entity =>
            {
                entity.ToTable("wallets");

                entity.HasIndex(a => a.Address)
                .HasName("index_wallets_address");

                entity.Property(a => a.Address)
                    .HasMaxLength(34);

                entity.HasKey(e => e.Id);
                
            });

            modelBuilder.Entity<Transactions>(entity =>
        {
            entity.ToTable("transactions");

            entity.HasIndex(e => e.Hash)
                .HasName("transactions_hash_index");        

            entity.HasIndex(e => e.Id)
                .HasName("partial_index_block_index")
                .HasFilter("((type)::text <> 'miner_transaction'::text)");

            entity.HasKey(a => a.Id);

            entity.Property(e => e.Id)
               .HasColumnName("id");
            
            entity.HasOne(a => a.Block)
            .WithMany(a => a.Transactions)
            .HasForeignKey(a => a.BlockId);

            entity.Property(e => e.Hash)
                .IsRequired()
                .HasColumnName("hash")
                .HasMaxLength(64);

            entity.Property(e => e.InsertedAt).HasColumnName("inserted_at");

            entity.Property(e => e.N).HasColumnName("n");

            entity.Property(e => e.NetFee)
                .HasColumnName("net_fee")
                .HasColumnType("numeric");

            entity.Property(e => e.Nonce).HasColumnName("nonce");

            entity.Property(e => e.Size).HasColumnName("size");

            entity.Property(e => e.SysFee)
                .HasColumnName("sys_fee")
                .HasColumnType("numeric");

            entity.Property(e => e.Type)
                .IsRequired()
                .HasColumnName("type")
                .HasMaxLength(255);

            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.Property(e => e.Version).HasColumnName("version");
        });

            modelBuilder.Entity<Transfers>(entity =>
            {
                entity.Property(a => a.Id)
                    .ValueGeneratedNever();

                entity.ToTable("transfers");


                //entity.HasIndex(e => e.TransactionId)
                //    .HasName("transfers_transaction_id_index");

                //entity.Property(e => e.AddressFrom)
                //    .IsRequired()
                //    .HasMaxLength(34)
                //    .HasColumnName("address_from");

                //entity.Property(e => e.AddressTo)
                //    .IsRequired()
                //    .HasMaxLength(34)
                //    .HasColumnName("address_to");




                entity.HasOne(a => a.AddressFrom)
                    .WithMany(b => b.TransfersOut)
                    .HasForeignKey(c => c.AddressFromId);

                entity.HasOne(a => a.AddressTo)
                    .WithMany(b => b.TransfersIn)
                    .HasForeignKey(c => c.AddressToId);

                //            .Entity<Student>()
                //.HasOne<StudentAddress>(s => s.Address)
                //.WithOne(ad => ad.Student)
                //.HasForeignKey<StudentAddress>(ad => ad.AddressOfStudentId);

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasColumnType("numeric");

                entity.Property(e => e.BlockIndex).HasColumnName("block_index");

                entity.Property(e => e.BlockTime).HasColumnName("block_time");

                //entity.Property(e => e.Contract)
                //    .IsRequired()
                //    .HasColumnName("contract");

                entity.Property(e => e.InsertedAt).HasColumnName("inserted_at");

                //entity.Property(e => e.TransactionId).HasColumnName("transaction_id");

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });
            
            modelBuilder.Entity<Assets>(entity =>
            {

                

                entity.ToTable("assets");

                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.Hash)
                .HasName("assets_hash_index");

                entity.Property(e => e.Hash)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.HasOne(e => e.AdminWallet);

                entity.HasOne(e => e.OwnerWallet);

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasColumnType("numeric");

                entity.Property(e => e.BlockTime).HasColumnName("block_time");                

                entity.Property(e => e.InsertedAt).HasColumnName("inserted_at");

                entity.Property(e => e.Issued)
                    .HasColumnName("issued")
                    .HasColumnType("numeric");

                //TODO: Verify max lenght
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(10);             

                entity.Property(e => e.Precision).HasColumnName("precision");

                entity.Property(e => e.Symbol)
                    .HasColumnName("symbol")
                    .HasMaxLength(255);

                entity.Property(e => e.TransactionId).HasColumnName("transaction_id");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnName("type")
                    .HasMaxLength(255);

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<Blocks>(entity =>
            {
                
                entity.ToTable("blocks");

                entity.Property(e => e.Hash)
                    .HasColumnName("blocks_pkey");

                entity.Property(e => e.Hash)
                .HasColumnName("hash")
                .HasMaxLength(64);

                entity.Property(e => e.CumulativeSysFee)
                    .HasColumnName("cumulative_sys_fee")
                    .HasColumnType("numeric");

                entity.Property(e => e.GasGenerated)
                    .HasColumnName("gas_generated")
                    .HasColumnType("numeric");

                entity.HasKey(e => e.Index)
                    .HasName("index");

                entity.Property(e => e.Index)
                .ValueGeneratedNever();

                entity.Property(e => e.InsertedAt).HasColumnName("inserted_at");

                entity.Property(e => e.MerkleRoot)
                    .IsRequired()
                    .HasColumnName("merkle_root")
                    .HasMaxLength(64);

                entity.Property(e => e.NextConsensus)
                    .IsRequired()
                    .HasColumnName("next_consensus");

                entity.Property(e => e.Nonce)
                    .IsRequired()
                    .HasColumnName("nonce");

                entity.Property(e => e.Script)
                    .IsRequired()
                    .HasColumnName("script")
                    .HasColumnType("jsonb");

                entity.Property(e => e.Size).HasColumnName("size");

                entity.Property(e => e.Time).HasColumnName("time");

                entity.Property(e => e.TotalNetFee)
                    .HasColumnName("total_net_fee")
                    .HasColumnType("numeric");

                entity.Property(e => e.TotalSysFee)
                    .HasColumnName("total_sys_fee")
                    .HasColumnType("numeric");

                entity.Property(e => e.TxCount).HasColumnName("tx_count");

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.Property(e => e.Version).HasColumnName("version");
            });


            //modelBuilder.Entity<AddressBalances>(entity =>
            //{
            //    entity.HasKey(e => new { e.AddressHash, e.AssetHash })
            //        .HasName("address_balances_pkey");

            //    entity.ToTable("address_balances");

            //    entity.Property(e => e.AddressHash).HasColumnName("address_hash");

            //    entity.Property(e => e.AssetHash).HasColumnName("asset_hash");

            //    entity.Property(e => e.InsertedAt).HasColumnName("inserted_at");

            //    entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            //    entity.Property(e => e.Value)
            //        .HasColumnName("value")
            //        .HasColumnType("numeric");
            //});

            //modelBuilder.Entity<AddressBalancesQueue>(entity =>
            //{
            //    entity.HasNoKey();

            //    entity.ToTable("address_balances_queue");

            //    entity.Property(e => e.AddressHash)
            //        .IsRequired()
            //        .HasColumnName("address_hash");

            //    entity.Property(e => e.AssetHash)
            //        .IsRequired()
            //        .HasColumnName("asset_hash");

            //    entity.Property(e => e.InsertedAt).HasColumnName("inserted_at");

            //    entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            //    entity.Property(e => e.Value)
            //        .HasColumnName("value")
            //        .HasColumnType("numeric");
            //});

            //modelBuilder.Entity<AddressHistories>(entity =>
            //{
            //    entity.HasNoKey();

            //    entity.ToTable("address_histories");

            //    entity.Property(e => e.AddressHash)
            //        .IsRequired()
            //        .HasColumnName("address_hash");

            //    entity.Property(e => e.AssetHash)
            //        .IsRequired()
            //        .HasColumnName("asset_hash");

            //    entity.Property(e => e.BlockTime).HasColumnName("block_time");

            //    entity.Property(e => e.InsertedAt).HasColumnName("inserted_at");

            //    entity.Property(e => e.TransactionId).HasColumnName("transaction_id");

            //    entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            //    entity.Property(e => e.Value)
            //        .HasColumnName("value")
            //        .HasColumnType("numeric");
            //});

            //modelBuilder.Entity<AddressTransactionBalances>(entity =>
            //{
            //    entity.HasKey(e => new { e.AddressHash, e.TransactionId, e.AssetHash })
            //        .HasName("address_transaction_balances_pkey");

            //    entity.ToTable("address_transaction_balances");

            //    entity.HasIndex(e => new { e.AddressHash, e.BlockTime })
            //        .HasName("address_transaction_balances_address_hash_block_time_index");

            //    entity.HasIndex(e => new { e.TransactionId, e.AssetHash })
            //        .HasName("address_transaction_balances_transaction_id_asset_hash_index");

            //    entity.Property(e => e.AddressHash).HasColumnName("address_hash");

            //    entity.Property(e => e.TransactionId).HasColumnName("transaction_id");

            //    entity.Property(e => e.AssetHash).HasColumnName("asset_hash");

            //    entity.Property(e => e.BlockTime).HasColumnName("block_time");

            //    entity.Property(e => e.InsertedAt).HasColumnName("inserted_at");

            //    entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            //    entity.Property(e => e.Value)
            //        .HasColumnName("value")
            //        .HasColumnType("numeric");
            //});

            //modelBuilder.Entity<AddressTransactionBalancesQueue>(entity =>
            //{
            //    entity.HasNoKey();

            //    entity.ToTable("address_transaction_balances_queue");

            //    entity.Property(e => e.AddressHash)
            //        .IsRequired()
            //        .HasColumnName("address_hash");

            //    entity.Property(e => e.AssetHash)
            //        .IsRequired()
            //        .HasColumnName("asset_hash");

            //    entity.Property(e => e.BlockTime).HasColumnName("block_time");

            //    entity.Property(e => e.InsertedAt).HasColumnName("inserted_at");

            //    entity.Property(e => e.TransactionId).HasColumnName("transaction_id");

            //    entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            //    entity.Property(e => e.Value)
            //        .HasColumnName("value")
            //        .HasColumnType("numeric");
            //});

            //modelBuilder.Entity<AddressTransactions>(entity =>
            //{
            //    entity.HasKey(e => new { e.AddressHash, e.TransactionId })
            //        .HasName("address_transactions_pkey");

            //    entity.ToTable("address_transactions");

            //    entity.Property(e => e.AddressHash).HasColumnName("address_hash");

            //    entity.Property(e => e.TransactionId).HasColumnName("transaction_id");

            //    entity.Property(e => e.BlockTime).HasColumnName("block_time");

            //    entity.Property(e => e.InsertedAt).HasColumnName("inserted_at");

            //    entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            //});

            //modelBuilder.Entity<Addresses>(entity =>
            //{
            //    entity.HasKey(e => e.Hash)
            //        .HasName("addresses_pkey");

            //    entity.ToTable("addresses");

            //    entity.HasIndex(e => e.LastTransactionTime)
            //        .HasName("addresses_last_transaction_time_index");

            //    entity.Property(e => e.Hash).HasColumnName("hash");

            //    entity.Property(e => e.AtbCount).HasColumnName("atb_count");

            //    entity.Property(e => e.FirstTransactionTime).HasColumnName("first_transaction_time");

            //    entity.Property(e => e.InsertedAt).HasColumnName("inserted_at");

            //    entity.Property(e => e.LastTransactionTime).HasColumnName("last_transaction_time");

            //    entity.Property(e => e.TxCount).HasColumnName("tx_count");

            //    entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            //});

            //modelBuilder.Entity<AddressesQueue>(entity =>
            //{
            //    entity.HasNoKey();

            //    entity.ToTable("addresses_queue");

            //    entity.Property(e => e.AtbCount).HasColumnName("atb_count");

            //    entity.Property(e => e.FirstTransactionTime).HasColumnName("first_transaction_time");

            //    entity.Property(e => e.Hash)
            //        .IsRequired()
            //        .HasColumnName("hash");

            //    entity.Property(e => e.InsertedAt).HasColumnName("inserted_at");

            //    entity.Property(e => e.LastTransactionTime).HasColumnName("last_transaction_time");

            //    entity.Property(e => e.TxCount).HasColumnName("tx_count");

            //    entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            //});


            //modelBuilder.Entity<BlocksMeta>(entity =>
            //{
            //    entity.ToTable("blocks_meta");

            //    entity.Property(e => e.Id)
            //        .HasColumnName("id")
            //        .ValueGeneratedNever();

            //    entity.Property(e => e.CumulativeSysFee)
            //        .HasColumnName("cumulative_sys_fee")
            //        .HasColumnType("numeric");

            //    entity.Property(e => e.Index).HasColumnName("index");
            //});

            //modelBuilder.Entity<BlocksQueue>(entity =>
            //{
            //    entity.HasKey(e => e.Index)
            //        .HasName("blocks_queue_pkey");

            //    entity.ToTable("blocks_queue");

            //    entity.Property(e => e.Index)
            //        .HasColumnName("index")
            //        .ValueGeneratedNever();

            //    entity.Property(e => e.TotalSysFee)
            //        .HasColumnName("total_sys_fee")
            //        .HasColumnType("numeric");
            //});

            //modelBuilder.Entity<Claims>(entity =>
            //{
            //    entity.HasNoKey();

            //    entity.ToTable("claims");

            //    entity.HasIndex(e => e.TransactionId)
            //        .HasName("claims_transaction_id_index");

            //    entity.HasIndex(e => new { e.VoutTransactionHash, e.VoutN })
            //        .HasName("claims_vout_transaction_hash_vout_n_index");

            //    entity.Property(e => e.BlockTime).HasColumnName("block_time");

            //    entity.Property(e => e.InsertedAt).HasColumnName("inserted_at");

            //    entity.Property(e => e.TransactionId).HasColumnName("transaction_id");

            //    entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            //    entity.Property(e => e.VoutN).HasColumnName("vout_n");

            //    entity.Property(e => e.VoutTransactionHash)
            //        .IsRequired()
            //        .HasColumnName("vout_transaction_hash");
            //});

            //modelBuilder.Entity<Counters>(entity =>
            //{
            //    entity.HasNoKey();

            //    entity.ToTable("counters");

            //    entity.Property(e => e.Name)
            //        .HasColumnName("name")
            //        .HasMaxLength(255);

            //    entity.Property(e => e.Ref).HasColumnName("ref");

            //    entity.Property(e => e.Value).HasColumnName("value");
            //});

            //modelBuilder.Entity<CountersCached>(entity =>
            //{
            //    entity.HasKey(e => new { e.Name, e.Ref })
            //        .HasName("counters_cached_pkey");

            //    entity.ToTable("counters_cached");

            //    entity.Property(e => e.Name)
            //        .HasColumnName("name")
            //        .HasMaxLength(255);

            //    entity.Property(e => e.Ref).HasColumnName("ref");

            //    entity.Property(e => e.Value).HasColumnName("value");
            //});

            //modelBuilder.Entity<CountersQueue>(entity =>
            //{
            //    entity.HasNoKey();

            //    entity.ToTable("counters_queue");

            //    entity.Property(e => e.Name)
            //        .IsRequired()
            //        .HasColumnName("name")
            //        .HasMaxLength(255);

            //    entity.Property(e => e.Ref)
            //        .IsRequired()
            //        .HasColumnName("ref");

            //    entity.Property(e => e.Value).HasColumnName("value");
            //});

            //modelBuilder.Entity<TransactionAssets>(entity =>
            //{
            //    entity.HasKey(e => new { e.TransactionId, e.AssetHash })
            //        .HasName("transaction_assets_pkey");

            //    entity.ToTable("transaction_assets");

            //    entity.Property(e => e.TransactionId).HasColumnName("transaction_id");

            //    entity.Property(e => e.AssetHash).HasColumnName("asset_hash");
            //});


            //modelBuilder.Entity<Vins>(entity =>
            //{
            //    entity.HasKey(e => new { e.VoutTransactionHash, e.VoutN })
            //        .HasName("vins_pkey");

            //    entity.ToTable("vins");

            //    entity.HasIndex(e => e.TransactionId)
            //        .HasName("vins_transaction_id_index");

            //    entity.Property(e => e.VoutTransactionHash).HasColumnName("vout_transaction_hash");

            //    entity.Property(e => e.VoutN).HasColumnName("vout_n");

            //    entity.Property(e => e.BlockIndex).HasColumnName("block_index");

            //    entity.Property(e => e.BlockTime).HasColumnName("block_time");

            //    entity.Property(e => e.InsertedAt).HasColumnName("inserted_at");

            //    entity.Property(e => e.N).HasColumnName("n");

            //    entity.Property(e => e.TransactionId).HasColumnName("transaction_id");

            //    entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            //});

            //modelBuilder.Entity<Vouts>(entity =>
            //{
            //    entity.HasKey(e => new { e.TransactionHash, e.N })
            //        .HasName("vouts_pkey");

            //    entity.ToTable("vouts");

            //    entity.HasIndex(e => e.AddressHash)
            //        .HasName("partial_vout_index");
            //        // TODO: DESCOBRIR ESSE INDEX
            //        //.HasFilter("((asset_hash = cast('xc56f33fc6ecfcd0c225c4ab356fee59390af8560be0e930faebe74a6daff7c9b' AS uuid)) AND (claimed = false))");

            //    entity.HasIndex(e => e.TransactionId)
            //        .HasName("vouts_transaction_id_index");

            //    entity.HasIndex(e => new { e.AddressHash, e.AssetHash })
            //        .HasName("vouts_address_hash_asset_hash_index");

            //    entity.HasIndex(e => new { e.AddressHash, e.Spent })
            //        .HasName("vouts_address_hash_spent_index");

            //    entity.HasIndex(e => new { e.AddressHash, e.Claimed, e.Spent })
            //        .HasName("vouts_address_hash_claimed_spent_index");

            //    entity.Property(e => e.TransactionHash).HasColumnName("transaction_hash");

            //    entity.Property(e => e.N).HasColumnName("n");

            //    entity.Property(e => e.AddressHash)
            //        .IsRequired()
            //        .HasColumnName("address_hash");

            //    entity.Property(e => e.AssetHash)
            //        .IsRequired()
            //        .HasColumnName("asset_hash");

            //    entity.Property(e => e.BlockTime).HasColumnName("block_time");

            //    entity.Property(e => e.Claimed).HasColumnName("claimed");

            //    entity.Property(e => e.EndBlockIndex).HasColumnName("end_block_index");

            //    entity.Property(e => e.InsertedAt).HasColumnName("inserted_at");

            //    entity.Property(e => e.Spent).HasColumnName("spent");

            //    entity.Property(e => e.StartBlockIndex).HasColumnName("start_block_index");

            //    entity.Property(e => e.TransactionId).HasColumnName("transaction_id");

            //    entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            //    entity.Property(e => e.Value)
            //        .HasColumnName("value")
            //        .HasColumnType("numeric");
            //});

            //modelBuilder.Entity<VoutsQueue>(entity =>
            //{
            //    entity.HasNoKey();

            //    entity.ToTable("vouts_queue");

            //    entity.Property(e => e.BlockTime).HasColumnName("block_time");

            //    entity.Property(e => e.Claimed).HasColumnName("claimed");

            //    entity.Property(e => e.EndBlockIndex).HasColumnName("end_block_index");

            //    entity.Property(e => e.InsertedAt).HasColumnName("inserted_at");

            //    entity.Property(e => e.N).HasColumnName("n");

            //    entity.Property(e => e.Spent).HasColumnName("spent");

            //    entity.Property(e => e.TransactionHash)
            //        .IsRequired()
            //        .HasColumnName("transaction_hash");

            //    entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            //    entity.Property(e => e.VinTransactionId).HasColumnName("vin_transaction_id");
            //});

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
