using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VeloBid.Services.Auctions.Domain.Aggregates;
using VeloBid.Services.Auctions.Domain.Entities;
using VeloBid.Services.Auctions.Domain.ValueObjects;

namespace VeloBid.Services.Auctions.Infrastructure.Persistence.Configurations;

internal sealed class AuctionConfiguration : IEntityTypeConfiguration<Auction>
{
    public void Configure(EntityTypeBuilder<Auction> builder)
    {
        builder.ToTable("auctions");

        builder.HasKey(auction => auction.Id);

        builder.Property(auction => auction.Id)
            .HasConversion(
                auctionId => auctionId.Value,
                value => AuctionId.From(value))
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(auction => auction.SellerId)
            .HasConversion(
                sellerId => sellerId.Value,
                value => UserId.From(value))
            .IsRequired()
            .HasColumnName("seller_id");

        builder.Property(auction => auction.Title)
            .HasMaxLength(200)
            .IsRequired()
            .HasColumnName("title");

        builder.Property(auction => auction.Description)
            .HasMaxLength(2_000)
            .IsRequired()
            .HasColumnName("description");

        builder.Property(auction => auction.Status)
            .HasConversion<int>()
            .IsRequired()
            .HasColumnName("status");

        builder.Property(auction => auction.CreatedAtUtc)
            .IsRequired()
            .HasColumnName("created_at_utc");

        builder.Property(auction => auction.ActivatedAtUtc)
            .HasColumnName("activated_at_utc");

        builder.Property(auction => auction.CompletedAtUtc)
            .HasColumnName("completed_at_utc");

        builder.Property(auction => auction.ExpiredAtUtc)
            .HasColumnName("expired_at_utc");

        builder.Property(auction => auction.WinnerId)
            .HasConversion(
                winnerId => winnerId.HasValue ? winnerId.Value.Value : (Guid?)null,
                value => value.HasValue ? UserId.From(value.Value) : null)
            .HasColumnName("winner_id");

        builder.ComplexProperty(auction => auction.StartingPrice, moneyBuilder =>
        {
            moneyBuilder.Property(money => money.Amount)
                .HasPrecision(18, 2)
                .IsRequired()
                .HasColumnName("starting_price_amount");

            moneyBuilder.Property(money => money.Currency)
                .HasMaxLength(3)
                .IsRequired()
                .HasColumnName("starting_price_currency");
        });

        builder.ComplexProperty(auction => auction.MinimumBidIncrement, moneyBuilder =>
        {
            moneyBuilder.Property(money => money.Amount)
                .HasPrecision(18, 2)
                .IsRequired()
                .HasColumnName("minimum_bid_increment_amount");

            moneyBuilder.Property(money => money.Currency)
                .HasMaxLength(3)
                .IsRequired()
                .HasColumnName("minimum_bid_increment_currency");
        });

        builder.ComplexProperty(auction => auction.Period, periodBuilder =>
        {
            periodBuilder.Property(period => period.StartsAtUtc)
                .IsRequired()
                .HasColumnName("starts_at_utc");

            periodBuilder.Property(period => period.EndsAtUtc)
                .IsRequired()
                .HasColumnName("ends_at_utc");
        });

        builder.ComplexProperty(auction => auction.WinningAmount, moneyBuilder =>
        {
            moneyBuilder.Property(money => money.Amount)
                .HasPrecision(18, 2)
                .HasColumnName("winning_amount");

            moneyBuilder.Property(money => money.Currency)
                .HasMaxLength(3)
                .HasColumnName("winning_currency");
        });

        builder.HasMany(auction => auction.Bids)
            .WithOne()
            .HasForeignKey(bid => bid.AuctionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(auction => auction.Bids)
            .HasField("_bids")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(auction => auction.CurrentPrice);

        builder.Ignore(auction => auction.HighestBid);

        builder.Ignore(auction => auction.DomainEvents);
    }
}