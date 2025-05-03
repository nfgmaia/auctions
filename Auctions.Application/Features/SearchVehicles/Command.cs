namespace Auctions.Application.Features.SearchVehicles;

public record Command
(
    string? Cursor,
    int? Limit,
    string? Manufacturer,
    string? Model,
    string? Type,
    int? Year
);