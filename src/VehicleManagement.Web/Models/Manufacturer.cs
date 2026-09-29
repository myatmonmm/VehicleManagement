using System.ComponentModel.DataAnnotations;

namespace VehicleManagement.Web.Models;

public class Manufacturer
{
    public int Id { get; set; }

    public required string Name {get; set;}
}
