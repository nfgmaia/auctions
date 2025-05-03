using Auctions.Domain.Entities;
using FluentValidation;

namespace Auctions.Application.Features.SearchVehicles;

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(c => c.Limit)
            .InclusiveBetween(1, 100)
            .When(v => v.Limit is not null);
        
        RuleFor(c => c.Manufacturer).MaximumLength(100).When(v => !string.IsNullOrWhiteSpace(v.Manufacturer));
        RuleFor(c => c.Model).MaximumLength(100).When(v => !string.IsNullOrWhiteSpace(v.Model));
        
        RuleFor(c => c.Type)
            .Must(v => Enum.TryParse<VehicleType>(v, true, out _))
            .WithMessage("Invalid vehicle type")
            .When(v => !string.IsNullOrWhiteSpace(v.Type));
        
        RuleFor(c => c.Year).InclusiveBetween(1885, 2099).When(v => v is not null);
    }
}