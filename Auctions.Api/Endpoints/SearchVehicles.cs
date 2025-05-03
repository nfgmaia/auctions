using Auctions.Application.Features.SearchVehicles;
using Auctions.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Auctions.Api.Endpoints;

internal static partial class EndpointExtensions
{
    public static void MapSearchVehicles(this IEndpointRouteBuilder app)
    {
        app.MapGet("/vehicles", async (
                [FromQuery] string? cursor,
                [FromQuery] int? limit,
                [FromQuery] string? manufacturer,
                [FromQuery] string? model,
                [FromQuery] string? type,
                [FromQuery] int? year,
                [FromServices] Handler handler) =>
            {
                var result = await handler.Handle(
                    new (cursor, limit, manufacturer, model, type, year));

                return result.IsSuccess ? 
                    Results.Ok(ToResponse(result.Value!, result.NextCursor, result.PreviousCursor)) : 
                    Results.BadRequest(result.Error);
            })
            .WithName("SearchVehicles");
    }
    
    // Note: Not recommended for production code, but used here for simplicity
    private static object ToResponse(IReadOnlyList<Vehicle> vehicles, string? nextCursor, string? previousCursor)
    {
        return new
        {
            Vehicles = vehicles.Select(object (v) => new
            {
                v.Id,
                v.Manufacturer,
                v.Model,
                Type = v.Type.ToString(),
                v.Year,
                NrDoors = v switch
                {
                    Hatchback h => h.NrDoors,
                    Sedan s => s.NrDoors,
                    _ => (int?)null
                },
                NrSeats = v switch
                {
                    Suv s => s.NrSeats,
                    _ => (int?)null
                },
                LoadCapacity = v switch
                {
                    Truck t => t.LoadCapacity,
                    _ => (int?)null
                }
            }).ToList(),
            nextCursor,
            previousCursor
        };
    }
}