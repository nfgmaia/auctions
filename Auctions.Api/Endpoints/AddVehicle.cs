using Auctions.Application.Features.AddVehicle;
using Auctions.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Auctions.Api.Endpoints;

internal static partial class EndpointExtensions
{
    public static void MapAddVehicle(this IEndpointRouteBuilder app)
    {
        app.MapPost("/vehicles", async (
                [FromBody] Command command,
                [FromServices] Handler handler) =>
            {
                var result = await handler.Handle(command);

                return result.IsSuccess ? 
                    Results.Created($"/vehicles/{result.Value?.Id}", result.Value!.ToResponse()) : 
                    Results.BadRequest(result.Error);
            })
            .WithName("AddVehicle");
    }
    
    // Note: Not recommended for production code, but used here for simplicity
    private static object ToResponse(this Vehicle vehicle)
    {
        return new
        {
            vehicle.Id,
            vehicle.Manufacturer,
            vehicle.Model,
            Type = vehicle.Type.ToString(),
            vehicle.Year,
            NrDoors = vehicle switch
            {
                Hatchback h => h.NrDoors,
                Sedan s => s.NrDoors,
                _ => (int?)null
            },
            NrSeats = vehicle switch
            {
                Suv s => s.NrSeats,
                _ => (int?)null
            },
            LoadCapacity = vehicle switch
            {
                Truck t => t.LoadCapacity,
                _ => (int?)null
            }
        };
    }
}