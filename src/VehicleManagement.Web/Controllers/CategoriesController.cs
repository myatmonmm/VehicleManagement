using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleManagement.Web.Data;
using VehicleManagement.Web.Models;
using VehicleManagement.Web.Services;

namespace VehicleManagement.Web.Controllers;

public class CategoriesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly CategoryService _categoryService;

    public CategoriesController(
        ApplicationDbContext context,
        CategoryService categoryService)
    {
        _context = context;
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _context.VehicleCategories
            .OrderBy(c => c.MinWeight)
            .ToListAsync();

        return View(categories);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VehicleCategory category)
    {
        if (!ModelState.IsValid)
        {
            return View(category);
        }

        var duplicateName = await _context.VehicleCategories
    .AnyAsync(c => c.Name == category.Name);

        if (duplicateName)
        {
            ModelState.AddModelError(
                nameof(category.Name),
                "A category with this name already exists.");

            return View(category);
        }

        var categories = await _context.VehicleCategories
            .OrderBy(c => c.MinWeight)
            .ToListAsync();

        var categoryToSplit = categories.SingleOrDefault(c =>
            category.MinWeight > c.MinWeight &&
            (c.MaxWeight == null || category.MinWeight < c.MaxWeight));

        if (categoryToSplit == null)
        {
            ModelState.AddModelError(
                nameof(category.MinWeight),
                "The new category must start inside an existing weight range.");

            return View(category);
        }

        category.MaxWeight = categoryToSplit.MaxWeight;
        categoryToSplit.MaxWeight = category.MinWeight;

        var updatedCategories = categories
            .Append(category)
            .ToList();

        var errors = _categoryService.ValidateRanges(updatedCategories);

        if (errors.Count > 0)
        {
            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(category);
        }

        _context.VehicleCategories.Add(category);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var category = await _context.VehicleCategories.FindAsync(id);

        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, VehicleCategory category)
    {
        if (id != category.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(category);
        }

        var duplicateName = await _context.VehicleCategories
    .AnyAsync(c => c.Id != id && c.Name == category.Name);

        if (duplicateName)
        {
            ModelState.AddModelError(
                nameof(category.Name),
                "A category with this name already exists.");

            return View(category);
        }

        var categories = await _context.VehicleCategories
            .OrderBy(c => c.MinWeight)
            .ToListAsync();

        var existing = categories.SingleOrDefault(c => c.Id == id);

        if (existing == null)
        {
            return NotFound();
        }

        var index = categories.IndexOf(existing);

        if (index == 0 && category.MinWeight != 0)
        {
            ModelState.AddModelError(
                nameof(category.MinWeight),
                "The first category must start at 0 kg.");

            return View(category);
        }

        if (index > 0)
        {
            categories[index - 1].MaxWeight = category.MinWeight;
        }

        if (index < categories.Count - 1)
        {
            if (category.MaxWeight == null)
            {
                ModelState.AddModelError(
                    nameof(category.MaxWeight),
                    "Only the final category can have no maximum weight.");

                return View(category);
            }

            categories[index + 1].MinWeight = category.MaxWeight.Value;
        }
        else
        {
            category.MaxWeight = null;
        }

        existing.Name = category.Name;
        existing.MinWeight = category.MinWeight;
        existing.MaxWeight = category.MaxWeight;
        existing.Icon = category.Icon;

        var errors = _categoryService.ValidateRanges(categories);

        if (errors.Count > 0)
        {
            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(category);
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var categories = await _context.VehicleCategories
            .OrderBy(c => c.MinWeight)
            .ToListAsync();

        var category = categories.SingleOrDefault(c => c.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        if (categories.Count == 1)
        {
            TempData["Error"] = "At least one category is required.";
            return RedirectToAction(nameof(Index));
        }

        var index = categories.IndexOf(category);

        if (index == 0)
        {
            // The next category takes over the deleted range.
            categories[index + 1].MinWeight = 0;
        }
        else
        {
            // The previous category takes over the deleted range.
            categories[index - 1].MaxWeight = category.MaxWeight;
        }

        _context.VehicleCategories.Remove(category);

        var remainingCategories = categories
            .Where(c => c.Id != id)
            .ToList();

        var errors = _categoryService.ValidateRanges(remainingCategories);

        if (errors.Count > 0)
        {
            TempData["Error"] = string.Join(" ", errors);
            return RedirectToAction(nameof(Index));
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}