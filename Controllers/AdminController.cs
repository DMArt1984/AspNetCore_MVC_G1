using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNetCore_MVC_Project.Data;
using AspNetCore_MVC_Project.ViewModels;
using AspNetCore_MVC_Project.Models.Control;

public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    // Конструктор контроллера, получает зависимость ApplicationDbContext (контекст базы данных)
    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Отображает страницу управления модулями (опциями) для конкретной компании.
    /// </summary>
    /// <param name="factoryId">Идентификатор компании</param>
    /// <returns>Представление с возможностью изменения доступных модулей</returns>
    public async Task<IActionResult> ManageOptions(int factoryId)
    {
        // Получаем данные о компании (фабрике) по ID
        var factory = await _context.Factories.FindAsync(factoryId);
        if (factory == null) return NotFound(); // Если компания не найдена, возвращаем 404

        // Загружаем все возможные модули (OptionBlocks), которые могут быть назначены
        var allModules = await _context.OptionBlocks.ToListAsync();

        // Определяем, какие модули уже назначены этой компании
        var assignedModules = await _context.Purchases
            .Where(p => p.FactoryId == factoryId)
            .Select(p => p.OptionBlockId)
            .ToListAsync();

        // Формируем модель представления для передачи данных в View
        var model = new OptionsViewModel
        {
            FactoryId = factory.Id, // ID компании
            FactoryTitle = factory.Title, // Название компании
            AllModules = allModules, // Все доступные модули
            AssignedModules = assignedModules // Уже назначенные модули
        };

        return View(model); // Передаём модель в представление
    }

    /// <summary>
    /// Обновляет список разрешённых модулей для компании (фабрики).
    /// </summary>
    /// <param name="model">Модель с обновлёнными данными</param>
    /// <returns>Перенаправление на страницу управления модулями</returns>
    [HttpPost]
    public async Task<IActionResult> UpdateOptions(OptionsViewModel model)
    {
        // Удаляем старые разрешения (удаляем все привязки модулей к компании)
        var existingOptions = _context.Purchases
            .Where(p => p.FactoryId == model.FactoryId);
        _context.Purchases.RemoveRange(existingOptions);

        // Если выбраны новые модули, добавляем их в базу
        if (model.SelectedModules != null)
        {
            var newOptions = model.SelectedModules
                .Select(moduleId => new Purchase
                {
                    FactoryId = model.FactoryId, // Привязываем модуль к компании
                    OptionBlockId = moduleId // ID нового модуля
                });

            _context.Purchases.AddRange(newOptions); // Добавляем новые записи в базу
        }

        await _context.SaveChangesAsync(); // Сохраняем изменения в базе
        return RedirectToAction("ManageOptions", new { factoryId = model.FactoryId }); // Перенаправляем пользователя обратно
    }
}

