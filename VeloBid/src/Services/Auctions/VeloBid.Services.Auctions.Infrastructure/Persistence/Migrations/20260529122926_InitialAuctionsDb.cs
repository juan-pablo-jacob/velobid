using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VeloBid.Services.Auctions.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialAuctionsDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "auctions");

            migrationBuilder.CreateTable(
                name: "auctions",
                schema: "auctions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    seller_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    activated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    completed_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    expired_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    winner_id = table.Column<Guid>(type: "uuid", nullable: true),
                    minimum_bid_increment_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    minimum_bid_increment_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    ends_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    starts_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    starting_price_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    starting_price_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    winning_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    winning_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auctions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "bids",
                schema: "auctions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    auction_id = table.Column<Guid>(type: "uuid", nullable: false),
                    bidder_id = table.Column<Guid>(type: "uuid", nullable: false),
                    placed_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bids", x => x.id);
                    table.ForeignKey(
                        name: "FK_bids_auctions_auction_id",
                        column: x => x.auction_id,
                        principalSchema: "auctions",
                        principalTable: "auctions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_bids_auction_id",
                schema: "auctions",
                table: "bids",
                column: "auction_id");

            migrationBuilder.CreateIndex(
                name: "ix_bids_auction_id_placed_at_utc",
                schema: "auctions",
                table: "bids",
                columns: new[] { "auction_id", "placed_at_utc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bids",
                schema: "auctions");

            migrationBuilder.DropTable(
                name: "auctions",
                schema: "auctions");
        }
    }
}
