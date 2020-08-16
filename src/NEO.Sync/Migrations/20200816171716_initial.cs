using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace NEO.Sync.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "blocks",
                columns: table => new
                {
                    index = table.Column<long>(nullable: false),
                    hash = table.Column<string>(maxLength: 64, nullable: true),
                    merkle_root = table.Column<string>(maxLength: 64, nullable: false),
                    next_consensus = table.Column<string>(nullable: false),
                    nonce = table.Column<string>(nullable: false),
                    script = table.Column<string>(type: "jsonb", nullable: false),
                    size = table.Column<int>(nullable: false),
                    time = table.Column<DateTime>(nullable: false),
                    version = table.Column<int>(nullable: false),
                    tx_count = table.Column<int>(nullable: false),
                    total_sys_fee = table.Column<decimal>(type: "numeric", nullable: false),
                    total_net_fee = table.Column<decimal>(type: "numeric", nullable: false),
                    cumulative_sys_fee = table.Column<decimal>(type: "numeric", nullable: true),
                    gas_generated = table.Column<decimal>(type: "numeric", nullable: false),
                    inserted_at = table.Column<DateTime>(nullable: false),
                    updated_at = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("index", x => x.index);
                });

            migrationBuilder.CreateTable(
                name: "wallets",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    address = table.Column<string>(maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_wallets", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "transactions",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    hash = table.Column<string>(maxLength: 64, nullable: false),
                    block_id = table.Column<long>(nullable: false),
                    net_fee = table.Column<decimal>(type: "numeric", nullable: false),
                    sys_fee = table.Column<decimal>(type: "numeric", nullable: false),
                    nonce = table.Column<long>(nullable: true),
                    size = table.Column<int>(nullable: false),
                    type = table.Column<string>(maxLength: 255, nullable: false),
                    version = table.Column<int>(nullable: false),
                    n = table.Column<int>(nullable: false),
                    inserted_at = table.Column<DateTime>(nullable: false),
                    updated_at = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transactions", x => x.id);
                    table.ForeignKey(
                        name: "fk_transactions_blocks_block_temp_id",
                        column: x => x.block_id,
                        principalTable: "blocks",
                        principalColumn: "index",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "assets",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    transaction_id = table.Column<long>(nullable: true),
                    hash = table.Column<string>(maxLength: 64, nullable: false),
                    admin_wallet_id = table.Column<long>(nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    name = table.Column<string>(maxLength: 10, nullable: false),
                    owner_wallet_id = table.Column<long>(nullable: false),
                    precision = table.Column<int>(nullable: false),
                    symbol = table.Column<string>(maxLength: 255, nullable: true),
                    type = table.Column<string>(maxLength: 255, nullable: false),
                    issued = table.Column<decimal>(type: "numeric", nullable: true),
                    inserted_at = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_assets", x => x.id);
                    table.ForeignKey(
                        name: "fk_assets_wallets_admin_wallet_id",
                        column: x => x.admin_wallet_id,
                        principalTable: "wallets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_assets_wallets_owner_wallet_id",
                        column: x => x.owner_wallet_id,
                        principalTable: "wallets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "transfers",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    address_from_id = table.Column<long>(nullable: false),
                    address_to_id = table.Column<long>(nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    block_index = table.Column<int>(nullable: false),
                    block_time = table.Column<DateTime>(nullable: false),
                    inserted_at = table.Column<DateTime>(nullable: false),
                    updated_at = table.Column<DateTime>(nullable: false),
                    asset_id = table.Column<long>(nullable: false),
                    transaction_id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transfers", x => x.id);
                    table.ForeignKey(
                        name: "FK_transfers_wallets_address_from_id",
                        column: x => x.address_from_id,
                        principalTable: "wallets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_transfers_wallets_address_to_id",
                        column: x => x.address_to_id,
                        principalTable: "wallets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_transfers_assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "assets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_transfers_transactions_transaction_id",
                        column: x => x.transaction_id,
                        principalTable: "transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_assets_admin_wallet_id",
                table: "assets",
                column: "admin_wallet_id");

            migrationBuilder.CreateIndex(
                name: "assets_hash_index",
                table: "assets",
                column: "hash");

            migrationBuilder.CreateIndex(
                name: "ix_assets_owner_wallet_id",
                table: "assets",
                column: "owner_wallet_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_block_id",
                table: "transactions",
                column: "block_id");

            migrationBuilder.CreateIndex(
                name: "transactions_hash_index",
                table: "transactions",
                column: "hash");

            migrationBuilder.CreateIndex(
                name: "partial_index_block_index",
                table: "transactions",
                column: "id",
                filter: "((type)::text <> 'miner_transaction'::text)");

            migrationBuilder.CreateIndex(
                name: "IX_transfers_address_from_id",
                table: "transfers",
                column: "address_from_id");

            migrationBuilder.CreateIndex(
                name: "IX_transfers_address_to_id",
                table: "transfers",
                column: "address_to_id");

            migrationBuilder.CreateIndex(
                name: "ix_transfers_asset_id",
                table: "transfers",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "ix_transfers_transaction_id",
                table: "transfers",
                column: "transaction_id");

            migrationBuilder.CreateIndex(
                name: "index_wallets_address",
                table: "wallets",
                column: "address");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "transfers");

            migrationBuilder.DropTable(
                name: "assets");

            migrationBuilder.DropTable(
                name: "transactions");

            migrationBuilder.DropTable(
                name: "wallets");

            migrationBuilder.DropTable(
                name: "blocks");
        }
    }
}
