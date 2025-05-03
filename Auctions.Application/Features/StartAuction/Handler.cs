using Auctions.Domain.Entities;
using Auctions.Domain.Ports;

namespace Auctions.Application.Features.StartAuction;

public class Handler(
    Validator validator, 
    IVehicleRepository vehicleRepository, 
    IAuctionRepository auctionRepository) 
    : IHandler<Command, Result<Auction>>
{
    public async Task<Result<Auction>> Handle(Command command)
    {
        // Validate command
        var validation = await validator.ValidateAsync(command);
        if (!validation.IsValid)
        {
            return Result<Auction>.Failure(
                validation.Errors.Select(e => e.ErrorMessage).First());
        }
        
        // Check if vehicle exists
        var vehicle = await vehicleRepository.GetAsync(command.VehicleId);
        if (vehicle == null)
        {
            return Result<Auction>.Failure("Vehicle not found");
        }

        // Check if vehicle is already in an active auction
        if (await auctionRepository.ExistsActiveAuctionForVehicleAsync(command.VehicleId))
        {
            return Result<Auction>.Failure("Vehicle is already in an active auction");
        }
        
        var auction = Auction.StartNew(command.VehicleId, vehicle.StartingBid);
        await auctionRepository.AddAsync(auction);

        return Result<Auction>.Success(auction);
    }
}