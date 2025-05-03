using Auctions.Domain.Entities;
using FluentValidation;

namespace Auctions.Application.Features.AddVehicle;

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(c => c.Manufacturer).NotEmpty().MaximumLength(100);
        RuleFor(c => c.Model).NotEmpty().MaximumLength(100);

        RuleFor(c => c.Type)
            .NotEmpty()
            .Must(v => Enum.TryParse(v, true, out VehicleType _))
            .WithMessage("'Type' is invalid");
        
        RuleFor(c => c.NrDoors)
            .NotEmpty()
            .InclusiveBetween(1, 10)
            .When(c => IsType(c, VehicleType.Hatchback, VehicleType.Sedan));

        RuleFor(c => c.NrSeats)
            .NotEmpty()
            .InclusiveBetween(1, 10)
            .When(c => IsType(c, VehicleType.Suv));

        RuleFor(c => c.LoadCapacity)
            .NotEmpty()
            .GreaterThan(0)
            .When(c => IsType(c, VehicleType.Truck));
        
        RuleFor(c => c.Year).NotEmpty().InclusiveBetween(1885, 2099);
        RuleFor(c => c.StartingBid).GreaterThanOrEqualTo(0);
    }
    
    private static bool IsType(Command command, params VehicleType[] types)
    {
        var type = Enum.Parse<VehicleType>(command.Type, true);
        return types.Contains(type);
    }
}