using Microsoft.EntityFrameworkCore;
using Readify.Models;

var builder = WebApplication.CreateBuilder(args);

// ======== SERVICES ========

// 1. MVC (Contrôleurs et Vues)
builder.Services.AddControllersWithViews();

// 2. Base de données (Connexion SQL Server)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? "Server=tonServeur;Database=taBaseDonne;Trusted_Connection=True;TrustServerCertificate=True;";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 3. Session (Pour le panier)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Durée de la session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 4. Authentification (Préparation)
builder.Services.AddAuthentication();

var app = builder.Build();

// ======== MIDDLEWARE (L'ordre est important !) ========

// Gestion des erreurs en production
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// IMPORTANT POUR LES IMAGES : Permet d'accéder au dossier wwwroot
app.UseStaticFiles();

app.UseRouting();

// Ordre critique : Session -> Auth -> Authorization
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Configuration des routes (URL)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
