using Auctions.Domain.Entities;
using NUnit.Framework;

namespace Auctions.UnitTests.Domain;

[TestFixture]
public class AuctionTests
{
    [Test]
    public void Create_WhenAllParametersAreValid_ReturnsAuction()
    {
        // Arrange
        var vehicleId = "vehicle123";
        var startingBid = 1000;
        var currentBid = 1500;
        var lastBidder = "user123";
        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddDays(7);
        var createdAt = DateTime.UtcNow;

        // Act
        var auction = Auction.Create(
            vehicleId,
            startDate,
            endDate,
            startingBid,
            currentBid,
            lastBidder,
            startDate,
            createdAt
        );

        // Assert
        Assert.That(auction, Is.Not.Null);
        Assert.That(auction.VehicleId, Is.EqualTo(vehicleId));
        Assert.That(auction.StartingBid, Is.EqualTo(startingBid));
        Assert.That(auction.CurrentBid, Is.EqualTo(currentBid));
        Assert.That(auction.LastBidder, Is.EqualTo(lastBidder));
        Assert.That(auction.StartDate, Is.EqualTo(startDate));
        Assert.That(auction.EndDate, Is.EqualTo(endDate));
        Assert.That(auction.CreatedAt, Is.EqualTo(createdAt));
    }

    [Test]
    public void StartNew_WhenParametersAreValid_ReturnsActiveAuction()
    {
        // Arrange
        var vehicleId = "vehicle123";
        var startingBid = 1000;

        // Act
        var auction = Auction.StartNew(vehicleId, startingBid);

        // Assert
        Assert.That(auction, Is.Not.Null);
        Assert.That(auction.VehicleId, Is.EqualTo(vehicleId));
        Assert.That(auction.StartingBid, Is.EqualTo(startingBid));
        Assert.That(auction.StartDate, Is.Not.Null);
        Assert.That(auction.EndDate, Is.Null);
        Assert.That(auction.CreatedAt, Is.Not.Null);
        Assert.That(auction.StartDate, Is.EqualTo(auction.CreatedAt));
        Assert.That(auction.IsActive, Is.True);
    }

    [Test]
    public void SetCurrentBid_WhenBidIsValid_UpdatesCurrentBidAndBidder()
    {
        // Arrange
        var auction = Auction.StartNew("vehicle123", 1000);
        var newBid = 1500;
        var bidder = "user123";

        // Act
        auction.SetCurrentBid(newBid, bidder);

        // Assert
        Assert.That(auction.CurrentBid, Is.EqualTo(newBid));
        Assert.That(auction.LastBidder, Is.EqualTo(bidder));
        Assert.That(auction.LastBidAt, Is.Not.Null);
    }

    [Test]
    public void SetCurrentBid_WhenBidIsLowerThanCurrentBid_ThrowsException()
    {
        // Arrange
        var auction = Auction.StartNew("vehicle123", 1000);
        auction.SetCurrentBid(1500, "user123");

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => auction.SetCurrentBid(1400, "user456"));
        Assert.That(exception?.ParamName, Is.EqualTo("bid"));
    }

    [Test]
    public void EndAuction_WhenAuctionIsActive_SetsEndDate()
    {
        // Arrange
        var auction = Auction.StartNew("vehicle123", 1000);

        // Act
        auction.EndAuction();

        // Assert
        Assert.That(auction.EndDate, Is.Not.Null);
        Assert.That(auction.IsActive, Is.False);
    }

    [Test]
    public void EndAuction_WhenAuctionIsAlreadyEnded_ThrowsException()
    {
        // Arrange
        var auction = Auction.StartNew("vehicle123", 1000);
        auction.EndAuction();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => auction.EndAuction());
        Assert.That(exception?.Message, Is.EqualTo("Auction already ended"));
    }
}