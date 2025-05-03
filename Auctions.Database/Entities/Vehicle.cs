using System.ComponentModel.DataAnnotations.Schema;
using Auctions.Domain.Entities;

namespace Auctions.Database.Entities;

[Table("vehicle")]
internal class Vehicle
{
   public required string Id { get; set; }
   public required string Manufacturer { get; set; }
   public required string Model { get; set; }
   public required VehicleType Type { get; set; }
   public required int Year { get; set; }
   public required long StartingBid { get; set; }
   public int? NrDoors { get; set; }
   public int? NrSeats { get; set; }
   public int? LoadCapacity { get; set; }
   public required DateTime CreatedAt { get; set; }
   
   public Domain.Entities.Vehicle ToDomainEntity()
   {
      VehicleProperties vehicleProperties = Type switch
      {
         VehicleType.Hatchback => new HatchbackProperties(Manufacturer, Model, Year, StartingBid, NrDoors ?? 0, Id),
         VehicleType.Sedan => new SedanProperties(Manufacturer, Model, Year, StartingBid, NrDoors ?? 0, Id),
         VehicleType.Suv => new SuvProperties(Manufacturer, Model, Year, StartingBid, NrSeats ?? 0, Id),
         VehicleType.Truck => new TruckProperties(Manufacturer, Model, Year, StartingBid, LoadCapacity ?? 0, Id),
         _ => throw new ArgumentOutOfRangeException(nameof(Type), Type, null)
      };

      return VehicleFactory.CreateVehicle(Type, vehicleProperties);
   }
   
   public static Vehicle FromDomainEntity(Domain.Entities.Vehicle vehicle)
   {
      return new()
      {
         Id = vehicle.Id!,
         Manufacturer = vehicle.Manufacturer!,
         Model = vehicle.Model!,
         Type = vehicle.Type,
         Year = vehicle.Year,
         StartingBid = vehicle.StartingBid,
         NrDoors = vehicle switch
         {
            Hatchback h => h.NrDoors,
            Sedan s => s.NrDoors,
            _ => null
         },
         NrSeats = vehicle is Suv suv ? suv.NrSeats : null,
         LoadCapacity = vehicle is Truck t ? t.LoadCapacity : null,
         CreatedAt = DateTime.UtcNow
      };
   }
}