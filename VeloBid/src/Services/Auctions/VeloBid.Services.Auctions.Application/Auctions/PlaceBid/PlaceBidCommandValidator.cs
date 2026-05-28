using FluentValidation;

namespace VeloBid.Services.Auctions.Application.Auctions.PlaceBid;

internal sealed class PlaceBidCommandValidator : AbstractValidator<PlaceBidCommand>
{
    public PlaceBidCommandValidator()
    {
        RuleFor(command => command.AuctionId)
            .NotEmpty();

        RuleFor(command => command.BidderId)
            .NotEmpty();

        RuleFor(command => command.Amount)
            .GreaterThan(0);

        RuleFor(command => command.Currency)
            .NotEmpty()
            .Length(3);
    }
}