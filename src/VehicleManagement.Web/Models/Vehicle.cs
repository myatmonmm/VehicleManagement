using System.ComponentModel.DataAnnotations;

namespace VehicleManagement.Web.Models;

public class Vehicle
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string OwnerName { get; set; } = string.Empty;

    [Required]
    public int ManufacturerId { get; set; }

    public Manufacturer? Manufacturer { get; set; }

    [Required]
    [CustomValidation(typeof(Vehicle), nameof(ValidateYear))]
    public int YearOfManufacture { get; set; }

    [Required]
    [CustomValidation(typeof(Vehicle), nameof(ValidateWeight))]
    public decimal WeightKg { get; set; }

    public static ValidationResult? ValidateYear(
     int year,
     ValidationContext context)
    {
        var maximumYear = DateTime.UtcNow.Year + 1;

        return year >= 1886 && year <= maximumYear
            ? ValidationResult.Success
            : new ValidationResult(
                $"Year must be between 1886 and {maximumYear}.",
                new[] { nameof(YearOfManufacture) });
    }

    public static ValidationResult? ValidateWeight(
      decimal weight,
      ValidationContext context)
    {
        if (weight <= 0)
        {
            return new ValidationResult(
                "Weight must be greater than zero.",
                new[] { nameof(WeightKg) });
        }

        if (decimal.Round(weight, 2) != weight)
        {
            return new ValidationResult(
                "Weight must have no more than two decimal places.",
                new[] { nameof(WeightKg) });
        }

        return ValidationResult.Success;
    }
}