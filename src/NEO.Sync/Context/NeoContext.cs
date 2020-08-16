using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NEO.Api.Models;
using NEO.Api.Worker.Extensions;
using System;

namespace NEO.Api.Data
{
    public partial class NeoContext : DbContext
    {
        private readonly IConfiguration configuration;

        public NeoContext(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public NeoContext(DbContextOptions<NeoContext> options)
            : base(options)
        {
        }

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
                    .HasMaxLength(36);

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
                entity.Property(a => a.Id);

                entity.ToTable("transfers");

                entity.HasOne(a => a.Transaction)
                    .WithMany(b => b.Transfers)
                    .HasForeignKey(c => c.TransactionId);

                entity.HasOne(a => a.AddressTo)
                    .WithMany(b => b.TransfersIn)
                    .HasForeignKey(c => c.AddressToId);

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasColumnType("numeric");

                entity.Property(e => e.BlockIndex).HasColumnName("block_index");

                entity.Property(e => e.BlockTime).HasColumnName("block_time");

                entity.Property(e => e.InsertedAt).HasColumnName("inserted_at");                

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

                //entity.Property(e => e.BlockTime).HasColumnName("block_time");

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

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
