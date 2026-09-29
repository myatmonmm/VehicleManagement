using VehicleManagement.Web.Models;

namespace VehicleManagement.Web.Services;

public class CategoryService
{
    public VehicleCategory GetCategoryForWeight(
        decimal weight,
        IEnumerable<VehicleCategory> categories)
    {
        var category = categories.SingleOrDefault(c =>
            weight >= c.MinWeight &&
            (c.MaxWeight == null || weight < c.MaxWeight));

        if (category == null)
        {
            throw new InvalidOperationException(
                $"No category is configured for weight {weight} kg.");
        }

        return category;
    }

    public List<string> ValidateRanges(IEnumerable<VehicleCategory> categories)
    {
        var orderedCategories = categories
            .OrderBy(c => c.MinWeight)
            .ToList();

        var errors = new List<string>();

        if (orderedCategories.Count == 0)
        {
            errors.Add("At least one category is required.");
            return errors;
        }

        if (orderedCategories[0].MinWeight != 0)
        {
            errors.Add("Category ranges must start at 0 kg.");
        }

        for (var i = 0; i < orderedCategories.Count; i++)
        {
            var current = orderedCategories[i];

            if (current.MaxWeight.HasValue &&
                current.MaxWeight.Value <= current.MinWeight)
            {
                errors.Add(
                    $"{current.Name} must have a maximum weight greater than its minimum weight.");
            }

            if (i < orderedCategories.Count - 1)
            {
                var next = orderedCategories[i + 1];

                if (current.MaxWeight == null)
                {
                    errors.Add(
                        $"{current.Name} cannot have an open-ended range before the final category.");
                    continue;
                }

                if (current.MaxWeight < next.MinWeight)
                {
                    errors.Add(
                        $"There is a gap between {current.Name} and {next.Name}.");
                }
                else if (current.MaxWeight > next.MinWeight)
                {
                    errors.Add(
                        $"{current.Name} and {next.Name} have overlapping ranges.");
                }
            }
            else if (current.MaxWeight != null)
            {
                errors.Add("The final category must have no maximum weight.");
            }
        }

        return errors;
    }
}
