using Auctions.Application.Features.StartAuction;
using Microsoft.AspNetCore.Mvc;

namespace Auctions.Api.Endpoints;

internal static partial class EndpointExtensions
{
    public static void MapStartAuction(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auctions", async (
                [FromBody] Command command,
                [FromServices] Handler handler) =>
            {
                var result = await handler.Handle(command);

                return result.IsSuccess ? 
                    Results.Created($"/auctions/{result.Value?.Id}", result.Value) : 
                    Results.BadRequest(result.Error);
            })
            .WithName("StartAuction");
    }
}