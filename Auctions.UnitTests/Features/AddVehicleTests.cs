using Auctions.Application.Features.AddVehicle;
using Auctions.Domain.Entities;
using Auctions.Domain.Ports;
using Moq;
using NUnit.Framework;

namespace Auctions.UnitTests.Features;

[TestFixture]
public class AddVehicleTests
{
    private Mock<IVehicleRepository> vehicleRepositoryMock = null!;
    private Validator validator = null!;
    private Handler handler = null!;
    
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        // Register vehicle factories
        VehicleFactory.RegisterFactory(VehicleType.Hatchback, new HatchbackFactory());
        VehicleFactory.RegisterFactory(VehicleType.Truck, new TruckFactory());
    }

    [SetUp]
    public void Setup()
    {
        vehicleRepositoryMock = new Mock<IVehicleRepository>();
        validator = new Validator();
        handler = new Handler(validator, vehicleRepositoryMock.Object);
    }
    
    [Test]
    public async Task Handle_WhenCommandIsValidForHatchback_ReturnsSuccess()
    {
        // Arrange
        var command = new Command
        (
            Manufacturer: "Toyota",
            Model: "Yaris",
            Type: "Hatchback",
            Year: 2020,
            StartingBid: 10000,
            NrDoors: 4,
            NrSeats: null,
            LoadCapacity: null
        );

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value, Is.InstanceOf<Hatchback>());
        Assert.That(result.Value!.Manufacturer, Is.EqualTo(command.Manufacturer));
        Assert.That(result.Value.Model, Is.EqualTo(command.Model));
        Assert.That(result.Value.Year, Is.EqualTo(command.Year));
        Assert.That(result.Value.StartingBid, Is.EqualTo(command.StartingBid));
        Assert.That(result.Value.Type, Is.EqualTo(VehicleType.Hatchback));
        Assert.That((result.Value as Hatchback)!.NrDoors, Is.EqualTo(command.NrDoors));
        vehicleRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Vehicle>()), Times.Once);
    }
    
    [Test]
    public async Task Handle_WhenCommandIsValidForTruck_ReturnsSuccess()
    {
        // Arrange
        var command = new Command
        (
            Manufacturer: "Ford",
            Model: "F-150",
            Type: "Truck",
            Year: 2021,
            StartingBid: 20000,
            NrDoors: null,
            NrSeats: null,
            LoadCapacity: 1000
        );

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value, Is.InstanceOf<Truck>());
        Assert.That(result.Value!.Manufacturer, Is.EqualTo(command.Manufacturer));
        Assert.That(result.Value.Model, Is.EqualTo(command.Model));
        Assert.That(result.Value.Year, Is.EqualTo(command.Year));
        Assert.That(result.Value.StartingBid, Is.EqualTo(command.StartingBid));
        Assert.That(result.Value.Type, Is.EqualTo(VehicleType.Truck));
        Assert.That((result.Value as Truck)!.LoadCapacity, Is.EqualTo(command.LoadCapacity));
        vehicleRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Vehicle>()), Times.Once);
    }
    
    [Test]
    public void Handle_WhenCommandHasInvalidVehicleType_ReturnsFailure()
    {
        // Arrange
        var command = new Command
        (
            Manufacturer: "Toyota",
            Model: "Yaris",
            Type: "InvalidType",
            Year: 2020,
            StartingBid: 10000,
            NrDoors: 4,
            NrSeats: null,
            LoadCapacity: null
        );

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () => await handler.Handle(command));
        Assert.That(exception?.Message, Is.EqualTo("Requested value 'InvalidType' was not found."));
    }

    [Test]
    public async Task Handle_WhenCommandIsMissingRequiredFields_ReturnsFailure()
    {
        // Arrange
        var command = new Command
        (
            Manufacturer: "",
            Model: "",
            Type: "Hatchback",
            Year: 0,
            StartingBid: 0,
            NrDoors: null,
            NrSeats: null,
            LoadCapacity: null
        );

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.Not.Empty);
        vehicleRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Vehicle>()), Times.Never);
    }

    [Test]
    public void Handle_WhenRepositoryThrowsException_ReturnsFailure()
    {
        // Arrange
        var command = new Command
        (
            Manufacturer: "Toyota",
            Model: "Yaris",
            Type: "Hatchback",
            Year: 2020,
            StartingBid: 10000,
            NrDoors: 4,
            NrSeats: null,
            LoadCapacity: null
        );

        vehicleRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Vehicle>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Asset
        var exception = Assert.ThrowsAsync<Exception>(async () => await handler.Handle(command));
        Assert.That(exception?.Message, Is.EqualTo("Database error"));
    }
    
    [OneTimeTearDown]
    public void OneTimeTeardown()
    {
        VehicleFactory.ClearFactories();
    }
}
