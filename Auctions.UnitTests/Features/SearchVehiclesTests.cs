using Auctions.Application.Features.SearchVehicles;
using Auctions.Domain.Entities;
using Auctions.Domain.Ports;
using Moq;
using NUnit.Framework;

namespace Auctions.UnitTests.Features;

[TestFixture]
public class SearchVehiclesTests
{
    
    private Mock<IVehicleRepository> vehicleRepositoryMock = null!;
    private Validator validator = null!;
    private Handler handler = null!;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        // Register vehicle factories
        VehicleFactory.RegisterFactory(VehicleType.Hatchback, new HatchbackFactory());
        VehicleFactory.RegisterFactory(VehicleType.Suv, new SuvFactory());
    }
    
    [SetUp]
    public void Setup()
    {
        vehicleRepositoryMock = new Mock<IVehicleRepository>();
        validator = new Validator();
        handler = new Handler(validator, vehicleRepositoryMock.Object);
    }

    [Test]
    public async Task Handle_WhenCommandIsValid_ReturnsPaginatedResult()
    {
        // Arrange
        var command = new Command(
            Cursor: null,
            Limit: 2,
            Manufacturer: "Toyota",
            Model: "Yaris",
            Type: "Hatchback",
            Year: null
        );
        
        var vehicles = new List<Vehicle>
        {
            VehicleFactory.CreateVehicle(VehicleType.Hatchback, new HatchbackProperties("Toyota", "Yaris", 2020, 13000, 5, "1")),
            VehicleFactory.CreateVehicle(VehicleType.Hatchback, new HatchbackProperties("Toyota", "Yaris", 2022, 16000, 5, "2")),
            VehicleFactory.CreateVehicle(VehicleType.Hatchback, new HatchbackProperties("Toyota", "Yaris", 2018, 11000, 5, "3")),
            VehicleFactory.CreateVehicle(VehicleType.Suv, new SuvProperties("Ford", "Kuga", 2019, 18000, 8, "4"))
        };
        var limit = command.Limit!.Value + 1;

        vehicleRepositoryMock
            .Setup(r => r.SearchAsync(null, true, limit, "Toyota", "Yaris", VehicleType.Hatchback, null))
            .ReturnsAsync(vehicles);

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value!.Count, Is.EqualTo(limit));
        Assert.That(result.NextCursor, Is.Not.Null);
        Assert.That(result.PreviousCursor, Is.Null);
    }

    [Test]
    public async Task Handle_WhenCommandHasInvalidType_ReturnsFailure()
    {
        // Arrange
        var command = new Command(
            Cursor: null,
            Limit: 2,
            Manufacturer: "Toyota",
            Model: "Yaris",
            Type: "InvalidType",
            Year: 2020
        );

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo("Invalid vehicle type"));
    }

    [Test]
    public async Task Handle_WhenRepositoryReturnsNoVehicles_ReturnsEmptyResult()
    {
        // Arrange
        var command = new Command(
            Cursor: null,
            Limit: 2,
            Manufacturer: "NonExistent",
            Model: "NonExistent",
            Type: "Hatchback",
            Year: 2020
        );
        
        var limit = command.Limit!.Value + 1;
        
        vehicleRepositoryMock
            .Setup(r => r.SearchAsync(null, true, limit, "NonExistent", "NonExistent", VehicleType.Hatchback, 2020))
            .ReturnsAsync([]);

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value!.Count, Is.EqualTo(0));
        Assert.That(result.NextCursor, Is.Null);
        Assert.That(result.PreviousCursor, Is.Null);
    }

    [Test]
    public void Handle_WhenCursorIsInvalid_ThrowsArgumentException()
    {
        // Arrange
        var command = new Command(
            Cursor: Convert.ToBase64String("InvalidCursor"u8.ToArray()),
            Limit: 2,
            Manufacturer: null,
            Model: null,
            Type: null,
            Year: null
        );

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () => await handler.Handle(command));
        Assert.That(exception?.Message, Is.EqualTo("Invalid cursor format"));
    }

    [Test]
    public async Task Handle_WhenValidationFails_ReturnsFailure()
    {
        // Arrange
        var command = new Command(
            Cursor: null,
            Limit: -1, // Invalid limit
            Manufacturer: null,
            Model: null,
            Type: null,
            Year: null
        );

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo("'Limit' must be between 1 and 100. You entered -1."));
    }
    
    [OneTimeTearDown]
    public void OneTimeTeardown()
    {
        VehicleFactory.ClearFactories();
    }
}