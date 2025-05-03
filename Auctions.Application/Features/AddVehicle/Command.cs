namespace Auctions.Application.Features.AddVehicle;

public record Command
(
    string Manufacturer,
    string Model,
    string Type,
    int Year,
    long StartingBid,
    int? NrDoors,
    int? NrSeats,
    int? LoadCapacity
);