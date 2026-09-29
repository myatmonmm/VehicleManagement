using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleManagement.Web.Data;
using VehicleManagement.Web.Models;
using VehicleManagement.Web.Services;

namespace VehicleManagement.Web.Controllers;

public class VehiclesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly CategoryService _categoryService;

    public VehiclesController(
        ApplicationDbContext context,
        CategoryService categoryService)
    {
        _context = context;
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index(string sortBy = "owner", string direction = "asc")
    {
        var query = _context.Vehicles
            .Include(v => v.Manufacturer)
            .AsQueryable();

        var descending = direction == "desc";

        query = sortBy.ToLower() switch
        {
            "manufacturer" => descending
                ? query.OrderByDescending(v => v.Manufacturer!.Name)
                : query.OrderBy(v => v.Manufacturer!.Name),

            "year" => descending
                ? query.OrderByDescending(v => v.YearOfManufacture)
                : query.OrderBy(v => v.YearOfManufacture),

            "weight" => descending
                ? query.OrderByDescending(v => v.WeightKg)
                : query.OrderBy(v => v.WeightKg),

            _ => descending
                ? query.OrderByDescending(v => v.OwnerName)
                : query.OrderBy(v => v.OwnerName)
        };

        var vehicles = await query.ToListAsync();

        var categories = await _context.VehicleCategories
            .OrderBy(c => c.MinWeight)
            .ToListAsync();

        var model = vehicles.Select(vehicle => new VehicleListItemViewModel
        {
            Vehicle = vehicle,
            Category = _categoryService.GetCategoryForWeight(
                vehicle.WeightKg,
                categories)
        }).ToList();

        ViewBag.SortBy = sortBy.ToLower();
        ViewBag.Direction = descending ? "desc" : "asc";

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Manufacturers = await _context.Manufacturers
            .OrderBy(m => m.Name)
            .ToListAsync();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Vehicle vehicle)
    {

        var manufacturerExists = await _context.Manufacturers
    .AnyAsync(m => m.Id == vehicle.ManufacturerId);

        if (!manufacturerExists)
        {
            ModelState.AddModelError(
                nameof(vehicle.ManufacturerId),
                "Select a valid manufacturer.");
        }
        if (!ModelState.IsValid)
        {
            ViewBag.Manufacturers = await _context.Manufacturers
                .OrderBy(m => m.Name)
                .ToListAsync();

            return View(vehicle);
        }

        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);

        if (vehicle == null)
        {
            return NotFound();
        }

        ViewBag.Manufacturers = await _context.Manufacturers
            .OrderBy(m => m.Name)
            .ToListAsync();

        return View(vehicle);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Vehicle vehicle)
    {
        if (id != vehicle.Id)
        {
            return NotFound();
        }

        var manufacturerExists = await _context.Manufacturers
            .AnyAsync(m => m.Id == vehicle.ManufacturerId);

        if (!manufacturerExists)
        {
            ModelState.AddModelError(
                nameof(vehicle.ManufacturerId),
                "Select a valid manufacturer.");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Manufacturers = await _context.Manufacturers
                .OrderBy(m => m.Name)
                .ToListAsync();

            return View(vehicle);
        }

        var existingVehicle = await _context.Vehicles.FindAsync(id);

        if (existingVehicle == null)
        {
            return NotFound();
        }

        existingVehicle.OwnerName = vehicle.OwnerName;
        existingVehicle.ManufacturerId = vehicle.ManufacturerId;
        existingVehicle.YearOfManufacture = vehicle.YearOfManufacture;
        existingVehicle.WeightKg = vehicle.WeightKg;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);

        if (vehicle == null)
        {
            return NotFound();
        }

        _context.Vehicles.Remove(vehicle);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}