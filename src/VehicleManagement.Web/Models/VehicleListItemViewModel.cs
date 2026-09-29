namespace VehicleManagement.Web.Models;

public class VehicleListItemViewModel
{
    public required Vehicle Vehicle { get; set; }

    public required VehicleCategory Category { get; set; }
}