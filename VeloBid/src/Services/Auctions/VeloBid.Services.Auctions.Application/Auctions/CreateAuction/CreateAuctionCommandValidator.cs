using FluentValidation;

namespace VeloBid.Services.Auctions.Application.Auctions.CreateAuction;

internal sealed class CreateAuctionCommandValidator : AbstractValidator<CreateAuctionCommand>
{
    public CreateAuctionCommandValidator()
    {
        RuleFor(command => command.SellerId)
            .NotEmpty();

        RuleFor(command => command.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(command => command.Description)
            .NotEmpty()
            .MaximumLength(2_000);

        RuleFor(command => command.StartingPrice)
            .GreaterThan(0);

        RuleFor(command => command.MinimumBidIncrement)
            .GreaterThan(0);

        RuleFor(command => command.Currency)
            .NotEmpty()
            .Length(3);

        RuleFor(command => command.EndsAtUtc)
            .GreaterThan(command => command.StartsAtUtc);
    }
}