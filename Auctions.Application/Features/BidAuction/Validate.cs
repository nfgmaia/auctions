using FluentValidation;

namespace Auctions.Application.Features.BidAuction;

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(c => c.AuctionId).NotEmpty();
        RuleFor(c => c.BidAmount).NotEmpty().GreaterThan(0);
        RuleFor(c => c.Bidder).NotEmpty().MaximumLength(100);
    }
}