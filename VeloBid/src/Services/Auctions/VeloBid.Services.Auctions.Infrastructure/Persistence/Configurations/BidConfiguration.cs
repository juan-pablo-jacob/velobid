using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VeloBid.Services.Auctions.Domain.Entities;
using VeloBid.Services.Auctions.Domain.ValueObjects;

namespace VeloBid.Services.Auctions.Infrastructure.Persistence.Configurations;

internal sealed class BidConfiguration : IEntityTypeConfiguration<Bid>
{
    public void Configure(EntityTypeBuilder<Bid> builder)
    {
        builder.ToTable("bids");

        builder.HasKey(bid => bid.Id);

        builder.Property(bid => bid.Id)
            .HasConversion(
                bidId => bidId.Value,
                value => BidId.From(value))
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(bid => bid.AuctionId)
            .HasConversion(
                auctionId => auctionId.Value,
                value => AuctionId.From(value))
            .IsRequired()
            .HasColumnName("auction_id");

        builder.Property(bid => bid.BidderId)
            .HasConversion(
                bidderId => bidderId.Value,
                value => UserId.From(value))
            .IsRequired()
            .HasColumnName("bidder_id");

        builder.ComplexProperty(bid => bid.Amount, moneyBuilder =>
        {
            moneyBuilder.Property(money => money.Amount)
                .HasPrecision(18, 2)
                .IsRequired()
                .HasColumnName("amount");

            moneyBuilder.Property(money => money.Currency)
                .HasMaxLength(3)
                .IsRequired()
                .HasColumnName("currency");
        });

        builder.Property(bid => bid.PlacedAtUtc)
            .IsRequired()
            .HasColumnName("placed_at_utc");

        builder.HasIndex(bid => bid.AuctionId)
            .HasDatabaseName("ix_bids_auction_id");

        builder.HasIndex(bid => new { bid.AuctionId, bid.PlacedAtUtc })
            .HasDatabaseName("ix_bids_auction_id_placed_at_utc");
    }
}