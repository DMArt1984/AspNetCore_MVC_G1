using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AspNetCore_MVC_Project.Data;
using AspNetCore_MVC_Project.Models;

var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Получение строки подключения из файла конфигурации appsettings.json.
/// "DefaultConnection" используется для подключения к основной базе данных.
/// </summary>
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var connectionCompanyMigrationString = builder.Configuration.GetConnectionString("CompanyDatabaseMigration");

/// <summary>
/// Добавление сервиса DbContext с использованием SQL Server.
/// ApplicationDbContext - основной контекст базы данных приложения, содержащий таблицы пользователей и компаний.
/// </summary>
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

/// <summary>
/// Настройка системы аутентификации и управления пользователями.
/// Используется Identity для работы с пользователями и ролями.
/// </summary>
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Добавляем `CompanyDbContext` для работы с миграциями
builder.Services.AddDbContext<CompanyDbContext>(options =>
    options.UseSqlServer(connectionCompanyMigrationString));


/// <summary>
/// Добавление MVC-контроллеров с поддержкой представлений.
/// </summary>
builder.Services.AddControllersWithViews();

/// <summary>
/// Регистрация UserManager и SignInManager, чтобы управлять пользователями.
/// UserManager - отвечает за создание и управление пользователями.
/// SignInManager - отвечает за вход и выход пользователей.
/// </summary>
builder.Services.AddScoped<UserManager<ApplicationUser>>();
builder.Services.AddScoped<SignInManager<ApplicationUser>>();

/// <summary>
/// Создание и настройка веб-приложения.
/// </summary>
var app = builder.Build();

// Убеждаемся, что таблицы существуют
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<CompanyDbContext>();
    dbContext.EnsureTablesCreated();
}

/// <summary>
/// Настройка обработки ошибок для продакшн-режима.
/// Если приложение работает не в режиме разработки, используется страница ошибки и HSTS.
/// </summary>
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Перенаправление на страницу ошибки
    app.UseHsts(); // Включение защиты с помощью HTTP Strict Transport Security (HSTS)
}

/// <summary>
/// Включение перенаправления всех HTTP-запросов в HTTPS.
/// </summary>
app.UseHttpsRedirection();

/// <summary>
/// Разрешение использования статических файлов (CSS, JS, изображения).
/// </summary>
app.UseStaticFiles();

/// <summary>
/// Включение маршрутизации.
/// </summary>
app.UseRouting();

/// <summary>
/// Включение системы аутентификации пользователей (проверка, вошел ли пользователь в систему).
/// </summary>
app.UseAuthentication();

/// <summary>
/// Включение системы авторизации (проверка прав пользователя на доступ к определенным ресурсам).
/// </summary>
app.UseAuthorization();

/// <summary>
/// Настройка маршрутов контроллеров.
/// Используется маршрут по умолчанию, который указывает, что начальной страницей будет "Home/Index".
/// </summary>
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

/// <summary>
/// Запуск веб-приложения.
/// </summary>
app.Run();