using Auctions.Domain.Entities;
using Auctions.Domain.Ports;

namespace Auctions.Application.Features.AddVehicle;

public class Handler(Validator validator, IVehicleRepository repository) 
    : IHandler<Command, Result<Vehicle>>
{
    public async Task<Result<Vehicle>> Handle(Command command)
    {
        // Validate command
        var validation = await validator.ValidateAsync(command);
        if (!validation.IsValid)
        {
            return Result<Vehicle>.Failure(
                validation.Errors.Select(e => e.ErrorMessage).First());
        }
        
        var type = Enum.Parse<VehicleType>(command.Type, true);

        // Create vehicle
        VehicleProperties vehicleProperties = type switch
        {
            VehicleType.Hatchback => new HatchbackProperties(
                command.Manufacturer, command.Model, command.Year, command.StartingBid, command.NrDoors ?? 0),
            VehicleType.Sedan => new SedanProperties(
                command.Manufacturer, command.Model, command.Year, command.StartingBid, command.NrDoors ?? 0),
            VehicleType.Suv => new SuvProperties(
                command.Manufacturer, command.Model, command.Year, command.StartingBid, command.NrSeats ?? 0),
            VehicleType.Truck => new TruckProperties(
                command.Manufacturer, command.Model, command.Year, command.StartingBid, command.LoadCapacity ?? 0),
            _ => throw new ArgumentOutOfRangeException(nameof(command), command.Type, null)
        };
        
        var vehicle = VehicleFactory.CreateVehicle(type, vehicleProperties);
        await repository.AddAsync(vehicle);
        
        return Result<Vehicle>.Success(vehicle);
    }
}