using Dapper;
using Microsoft.EntityFrameworkCore;
using VeloBid.Services.Auctions.Application.Abstractions.Persistence;
using VeloBid.Services.Auctions.Application.Auctions.GetActiveAuctions;
using VeloBid.Services.Auctions.Application.Auctions.GetAuctionById;
using VeloBid.Services.Auctions.Domain.ValueObjects;

namespace VeloBid.Services.Auctions.Infrastructure.Persistence.Repositories;

internal sealed class AuctionReadRepository(AuctionsDbContext dbContext) : IAuctionReadRepository
{
    public async Task<AuctionDetailsDto?> GetByIdAsync(
        AuctionId auctionId,
        CancellationToken cancellationToken = default)
    {
        var connection = dbContext.Database.GetDbConnection();

        const string sql = @"
                           SELECT
                               a.id AS Id,
                               a.seller_id AS SellerId,
                               a.title AS Title,
                               a.description AS Description,
                               a.status AS Status,
                               a.starting_price_amount AS StartingPrice,
                               COALESCE(MAX(b.amount), a.starting_price_amount) AS CurrentPrice,
                               a.starting_price_currency AS Currency,
                               a.starts_at_utc AS StartsAtUtc,
                               a.ends_at_utc AS EndsAtUtc,
                               a.winner_id AS WinnerId,
                               a.winning_amount AS WinningAmount
                           FROM auctions.auctions a
                           LEFT JOIN auctions.bids b ON b.auction_id = a.id
                           WHERE a.id = @AuctionId
                           GROUP BY
                               a.id,
                               a.seller_id,
                               a.title,
                               a.description,
                               a.status,
                               a.starting_price_amount,
                               a.starting_price_currency,
                               a.starts_at_utc,
                               a.ends_at_utc,
                               a.winner_id,
                               a.winning_amount";

        return await connection.QuerySingleOrDefaultAsync<AuctionDetailsDto>(
            new CommandDefinition(
                sql,
                new { AuctionId = auctionId.Value },
                cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyCollection<AuctionSummaryDto>> GetActiveAuctionsAsync(
        CancellationToken cancellationToken = default)
    {
        var connection = dbContext.Database.GetDbConnection();

        const string sql = @"
            SELECT
                a.id AS Id,
                a.title AS Title,
                a.status AS Status,
                COALESCE(MAX(b.amount), a.starting_price_amount) AS CurrentPrice,
                a.starting_price_currency AS Currency,
                a.ends_at_utc AS EndsAtUtc,
                COUNT(b.id)::int AS TotalBids
            FROM auctions.auctions a
            LEFT JOIN auctions.bids b ON b.auction_id = a.id
            WHERE a.status = 2
            GROUP BY
                a.id,
                a.title,
                a.status,
                a.starting_price_amount,
                a.starting_price_currency,
                a.ends_at_utc
            ORDER BY a.ends_at_utc ASC
            ";

        var auctions = await connection.QueryAsync<AuctionSummaryDto>(
            new CommandDefinition(
                sql,
                cancellationToken: cancellationToken));

        return auctions.AsList();
    }
}