using System.ComponentModel.DataAnnotations;

namespace VehicleManagement.Web.Models;

public class VehicleCategory
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Minimum weight cannot be negative.")]
    public decimal MinWeight { get; set; }

    public decimal? MaxWeight { get; set; }

    [Required]
    [MaxLength(100)]
    public string Icon { get; set; } = string.Empty;
}