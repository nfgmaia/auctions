using FluentValidation;

namespace Auctions.Application.Features.EndAuction;

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(c => c.AuctionId).NotEmpty();
    }
}