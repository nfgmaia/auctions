using Auctions.Application.Features.BidAuction;
using Microsoft.AspNetCore.Mvc;

namespace Auctions.Api.Endpoints;

internal static partial class EndpointExtensions
{
    public static void MapBidAuction(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auctions/{auctionId}/bid", async (
                [FromRoute] string auctionId,
                [FromBody] BidAuctionRequestBody body,
                [FromServices] Handler handler) =>
            {
                var result = await handler.Handle(
                    new (auctionId, body.BidAmount, body.Bidder));

                return result.IsSuccess ? 
                    Results.Ok(result.Value) : 
                    Results.BadRequest(result.Error);
            })
            .WithName("BidAuction");
    }
}

internal record BidAuctionRequestBody(long BidAmount, string Bidder);
