using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNetCore_MVC_Project.Data;
using AspNetCore_MVC_Project.ViewModels;
using AspNetCore_MVC_Project.Models.Control;

public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> ManageOptions(int factoryId)
    {
        var factory = await _context.Factories.FindAsync(factoryId);
        if (factory == null) return NotFound();

        var allModules = await _context.OptionBlocks.ToListAsync();
        var assignedModules = await _context.Purchases
            .Where(p => p.FactoryId == factoryId)
            .Select(p => p.OptionBlockId)
            .ToListAsync();

        var model = new OptionsViewModel
        {
            FactoryId = factory.Id,
            FactoryName = factory.Title,
            AllModules = allModules,
            AssignedModules = assignedModules
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateOptions(OptionsViewModel model)
    {
        var existingOptions = _context.Purchases
            .Where(p => p.FactoryId == model.FactoryId);
        _context.Purchases.RemoveRange(existingOptions); // Удаляем старые разрешения

        if (model.SelectedModules != null)
        {
            var newOptions = model.SelectedModules
                .Select(moduleId => new Purchase
                {
                    FactoryId = model.FactoryId,
                    OptionBlockId = moduleId
                });

            _context.Purchases.AddRange(newOptions);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("ManageOptions", new { factoryId = model.FactoryId });
    }
}
