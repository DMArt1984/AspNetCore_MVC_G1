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
    /// Загружаются все доступные модули (OptionBlocks) вместе с информацией об области, 
    /// а также определяется, какие модули уже назначены компании.
    /// </summary>
    /// <param name="factoryId">Идентификатор компании</param>
    /// <returns>Представление с возможностью изменения доступных модулей</returns>
    public async Task<IActionResult> ManageOptions(int factoryId)
    {
        // Получаем данные о компании (фабрике) по ID
        var factory = await _context.Factories.FindAsync(factoryId);
        if (factory == null)
            return NotFound(); // Если компания не найдена, возвращаем 404

        // Загружаем все возможные модули (OptionBlocks) с информацией об области.
        // Например, можно упорядочить модули сначала по области, затем по имени контроллера.
        var allModules = await _context.OptionBlocks
            .OrderBy(ob => ob.NameArea)
            .ThenBy(ob => ob.NameController)
            .ToListAsync();

        // Определяем, какие модули уже назначены этой компании.
        // Здесь выбираем только идентификаторы модулей, назначенных компании.
        var assignedModules = await _context.Purchases
            .Where(p => p.FactoryId == factoryId)
            .Select(p => p.OptionBlockId)
            .ToListAsync();

        // Формируем модель представления для передачи данных в View.
        // В модель передаются: идентификатор и название компании, список всех модулей и список назначенных модулей.
        var model = new OptionsViewModel
        {
            FactoryId = factory.Id,          // ID компании
            FactoryTitle = factory.Title,      // Название компании
            AllModules = allModules,           // Все доступные модули с информацией об области
            AssignedModules = assignedModules  // Уже назначенные модули (OptionBlockId)
        };

        return View(model); // Передаем модель в представление
    }

    /// <summary>
    /// Обновляет список разрешенных модулей для компании (фабрики).
    /// Удаляются старые привязки модулей, затем добавляются новые, выбранные администратором.
    /// </summary>
    /// <param name="model">Модель с обновленными данными</param>
    /// <returns>Перенаправление на страницу управления модулями</returns>
    [HttpPost]
    public async Task<IActionResult> UpdateOptions(OptionsViewModel model)
    {
        // Удаляем старые разрешения (удаляем все записи о назначенных модулях для данной компании)
        var existingOptions = _context.Purchases
            .Where(p => p.FactoryId == model.FactoryId);
        _context.Purchases.RemoveRange(existingOptions);

        // Если выбраны новые модули, добавляем их в базу данных
        if (model.SelectedModules != null)
        {
            var newOptions = model.SelectedModules
                .Select(moduleId => new Purchase
                {
                    FactoryId = model.FactoryId,   // Привязываем модуль к компании
                    OptionBlockId = moduleId        // ID выбранного модуля
                });

            _context.Purchases.AddRange(newOptions); // Добавляем новые записи
        }

        await _context.SaveChangesAsync(); // Сохраняем изменения в БД

        // Перенаправляем пользователя обратно на страницу управления модулями для данной компании
        return RedirectToAction("ManageOptions", new { factoryId = model.FactoryId });
    }
}
