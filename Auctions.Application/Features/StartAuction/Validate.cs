using FluentValidation;

namespace Auctions.Application.Features.StartAuction;

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(c => c.VehicleId).NotEmpty();
    }
}