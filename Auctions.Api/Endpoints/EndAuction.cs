using Auctions.Application.Features.EndAuction;
using Microsoft.AspNetCore.Mvc;

namespace Auctions.Api.Endpoints;

internal static partial class EndpointExtensions
{
    public static void MapEndAuction(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auctions/{auctionId}/end", async (
                [FromRoute] string auctionId,
                [FromServices] Handler handler) =>
            {
                var result = await handler.Handle(new (auctionId));

                return result.IsSuccess ? 
                    Results.Ok(result.Value) : 
                    Results.BadRequest(result.Error);
            })
            .WithName("EndAuction");
    }
}