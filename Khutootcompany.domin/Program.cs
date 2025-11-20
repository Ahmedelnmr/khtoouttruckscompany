using Khutootcompany.Domain.Interfaces;
using Khutootcompany.Application.Interfaces;
using Khutootcompany.Application.Services;
using Khutootcompany.Infrastructure.Data;
using Khutootcompany.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Khutootcompany.domin
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllersWithViews()
                .AddRazorRuntimeCompilation(); // For development

            // Database
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("Khutootcompany.Infrastructure")));

            // Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 4;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 5;

                // User settings
                options.User.RequireUniqueEmail = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Cookie settings
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.SlidingExpiration = true;
            });

            // HttpContextAccessor for Audit
            builder.Services.AddHttpContextAccessor();

            // Dependency Injection
            // Infrastructure
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Application Services
            builder.Services.AddScoped<ITruckService, TruckService>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddScoped<IInstallmentService, InstallmentService>();
            builder.Services.AddScoped<ICashTransactionService, CashTransactionService>();
            builder.Services.AddScoped<IWakalaService, WakalaService>();
            builder.Services.AddScoped<IVisitCardService, VisitCardService>();
            builder.Services.AddScoped<IInsuranceRenewalService, InsuranceRenewalService>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();
            builder.Services.AddScoped<IReportService, ReportService>();
            builder.Services.AddScoped<IAuditService, AuditService>();

            // AutoMapper (if using)
            // builder.Services.AddAutoMapper(typeof(Program));

            // Session (if needed)
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Seed Database
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    // Apply migrations
                    await context.Database.MigrateAsync();

                    // Seed roles
                    if (!await roleManager.RoleExistsAsync("Admin"))
                        await roleManager.CreateAsync(new IdentityRole("Admin"));

                    if (!await roleManager.RoleExistsAsync("User"))
                        await roleManager.CreateAsync(new IdentityRole("User"));

                    // Seed default admin
                    if (await userManager.FindByNameAsync("admin") == null)
                    {
                        var admin = new ApplicationUser
                        {
                            UserName = "admin",
                            FullName = "?????? ?????",
                            EmailConfirmed = true
                        };

                        var result = await userManager.CreateAsync(admin, "Admin@123");
                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(admin, "Admin");
                        }
                    }

                   
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
