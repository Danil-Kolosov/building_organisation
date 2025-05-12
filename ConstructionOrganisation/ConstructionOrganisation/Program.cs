using ConstructionOrganisation.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Сервисы
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Добавьте это перед всеми другими сервисами
builder.Services.AddDistributedMemoryCache(); // Необходимо для работы сессий
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<MySqlAuthService>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

var app = builder.Build();

// Middleware pipeline
app.UseStaticFiles();
app.UseRouting();
app.UseSession(); // Должно быть после UseRouting()
app.MapRazorPages();
app.MapControllers();

app.Run();




//using ConstructionOrganisation.Data;
//using Microsoft.EntityFrameworkCore;

//var builder = WebApplication.CreateBuilder(args);

//// Сервисы
//builder.Services.AddControllersWithViews();
//builder.Services.AddRazorPages();
//builder.Services.AddSession();
//builder.Services.AddScoped<MySqlAuthService>();
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseMySql(
//        builder.Configuration.GetConnectionString("DefaultConnection"),
//        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
//    ));

//var app = builder.Build();
//// Middleware
//app.UseStaticFiles();
//app.UseRouting();
//app.UseSession();
//app.MapRazorPages();
//app.MapControllers();

//app.Run();






/*using ConstructionOrganisation.Data;
//using ConstructionOrganisation.Data.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Добавляем поддержку сессий (обязательно ДО добавления контроллеров)
builder.Services.AddDistributedMemoryCache(); // Хранилище для сессий в памяти
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 2. Подключение к MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 3. Регистрация сервисов
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor(); // Для доступа к HttpContext
builder.Services.AddScoped<MySqlAuthService>();// Ваш сервис аутентификации
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.IsEssential = true;
}); 

var app = builder.Build();

// 4. Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

// 5. Подключение сессий (обязательно после UseRouting и до UseEndpoints)
app.UseSession();

app.UseAuthentication(); // Если используете Identity
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "reports",
    pattern: "Reports/{action=Engineering}/{managementId?}",
    defaults: new { controller = "Report" });

app.MapRazorPages();

app.Run();
*/


/*


using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ConstructionOrganisation.Data;
using ConstructionOrganisation.Service;
using System;

var builder = WebApplication.CreateBuilder(args);


// Добавьте это перед любыми другими настройками
builder.Services.AddDbContext<BuildingOrganisationContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()));

// Для Identity (если используется)
builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddEntityFrameworkStores<BuildingOrganisationContext>();

// Для Identity (если используете):
//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<BuildingOrganisationContext>();



// Подключение к БД
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(connectionString));

// Настройка Identity с авторизацией по логину
builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = false; // Не требовать email
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();

// Регистрируем сервис аутентификации
builder.Services.AddScoped<MySqlAuthService>();

builder.Services.AddControllers();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.Run();



/*using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ConstructionOrganisation.Data;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();

// Настройка авторизации (если выбрали Individual Accounts)
builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.SignIn.RequireConfirmedAccount = false; // Отключаем подтверждение почты
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"; // Разрешенные символы
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
*/