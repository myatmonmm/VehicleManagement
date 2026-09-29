using System.ComponentModel.DataAnnotations;
using VehicleManagement.Web.Models;

namespace VehicleManagement.Tests;

public class VehicleValidationTests
{
    [Fact]
    public void Validate_InvalidYear_ReturnsValidationError()
    {
        var vehicle = CreateValidVehicle();
        vehicle.YearOfManufacture = 1800;

        var results = Validate(vehicle);

        Assert.Contains(
            results,
            r => r.MemberNames.Contains(nameof(Vehicle.YearOfManufacture)));
    }

    [Fact]
    public void Validate_NegativeWeight_ReturnsValidationError()
    {
        var vehicle = CreateValidVehicle();
        vehicle.WeightKg = -100m;

        var results = Validate(vehicle);

        Assert.Contains(
            results,
            r => r.MemberNames.Contains(nameof(Vehicle.WeightKg)));
    }

    [Fact]
    public void Validate_WeightWithMoreThanTwoDecimalPlaces_ReturnsValidationError()
    {
        var vehicle = CreateValidVehicle();
        vehicle.WeightKg = 123.456m;

        var results = Validate(vehicle);

        Assert.Contains(
            results,
            r => r.MemberNames.Contains(nameof(Vehicle.WeightKg)));
    }

    [Fact]
    public void Validate_ValidVehicle_ReturnsNoValidationErrors()
    {
        var vehicle = CreateValidVehicle();

        var results = Validate(vehicle);

        Assert.Empty(results);
    }

    private static Vehicle CreateValidVehicle()
    {
        return new Vehicle
        {
            OwnerName = "Test Owner",
            ManufacturerId = 1,
            YearOfManufacture = 2025,
            WeightKg = 750.50m
        };
    }

    private static List<ValidationResult> Validate(Vehicle vehicle)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(vehicle);

        Validator.TryValidateObject(
            vehicle,
            context,
            results,
            validateAllProperties: true);

        return results;
    }
}