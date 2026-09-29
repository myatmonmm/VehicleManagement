using VehicleManagement.Web.Models;
using VehicleManagement.Web.Services;

namespace VehicleManagement.Tests;

public class CategoryServiceTests
{
    private readonly CategoryService _service = new();

    private readonly List<VehicleCategory> _categories =
    [
        new VehicleCategory
        {
            Id = 1,
            Name = "Light",
            MinWeight = 0m,
            MaxWeight = 500m,
            Icon = "car-light"
        },
        new VehicleCategory
        {
            Id = 2,
            Name = "Medium",
            MinWeight = 500m,
            MaxWeight = 2500m,
            Icon = "car-medium"
        },
        new VehicleCategory
        {
            Id = 3,
            Name = "Heavy",
            MinWeight = 2500m,
            MaxWeight = null,
            Icon = "truck"
        }
    ];

    [Theory]
    [InlineData(499.99, "Light")]
    [InlineData(500.00, "Medium")]
    [InlineData(2499.99, "Medium")]
    [InlineData(2500.00, "Heavy")]
    public void GetCategoryForWeight_ReturnsCorrectCategory(
        decimal weight,
        string expectedCategory)
    {
        var result = _service.GetCategoryForWeight(weight, _categories);

        Assert.Equal(expectedCategory, result.Name);
    }


    [Fact]
    public void ValidateRanges_ReturnsNoErrors_ForValidRanges()
    {
        var errors = _service.ValidateRanges(_categories);

        Assert.Empty(errors);
    }

    [Fact]
    public void ValidateRanges_ReturnsError_WhenThereIsGap()
    {
        var categories = new List<VehicleCategory>
    {
        new()
        {
            Name = "Light",
            MinWeight = 0m,
            MaxWeight = 400m,
            Icon = "car-light"
        },
        new()
        {
            Name = "Medium",
            MinWeight = 500m,
            MaxWeight = null,
            Icon = "car-medium"
        }
    };

        var errors = _service.ValidateRanges(categories);

        Assert.Contains(errors, error => error.Contains("gap"));
    }

    [Fact]
    public void ValidateRanges_ReturnsError_WhenRangesOverlap()
    {
        var categories = new List<VehicleCategory>
    {
        new()
        {
            Name = "Light",
            MinWeight = 0m,
            MaxWeight = 600m,
            Icon = "car-light"
        },
        new()
        {
            Name = "Medium",
            MinWeight = 500m,
            MaxWeight = null,
            Icon = "car-medium"
        }
    };

        var errors = _service.ValidateRanges(categories);

        Assert.Contains(errors, error => error.Contains("overlapping"));
    }

    [Fact]
    public void GetCategoryForWeight_WhenRangesChange_ReturnsUpdatedCategory()
    {
        var service = new CategoryService();

        var originalCategories = new List<VehicleCategory>
    {
        new() { Id = 1, Name = "Light", MinWeight = 0m, MaxWeight = 500m, Icon = "car" },
        new() { Id = 2, Name = "Medium", MinWeight = 500m, MaxWeight = 2500m, Icon = "car" },
        new() { Id = 3, Name = "Heavy", MinWeight = 2500m, MaxWeight = null, Icon = "truck" }
    };

        var updatedCategories = new List<VehicleCategory>
    {
        new() { Id = 1, Name = "Light", MinWeight = 0m, MaxWeight = 1000m, Icon = "car" },
        new() { Id = 3, Name = "Heavy", MinWeight = 1000m, MaxWeight = null, Icon = "truck" }
    };

        var originalCategory =
            service.GetCategoryForWeight(750m, originalCategories);

        var updatedCategory =
            service.GetCategoryForWeight(750m, updatedCategories);

        Assert.Equal("Medium", originalCategory.Name);
        Assert.Equal("Light", updatedCategory.Name);
    }
}