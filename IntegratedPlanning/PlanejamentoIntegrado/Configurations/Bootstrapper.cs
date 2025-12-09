using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PlanejamentoIntegrado.Configurations;
using PlanejamentoIntegrado.Data;
using PlanejamentoIntegrado.Models;
using PlanejamentoIntegrado.Repositories;
using PlanejamentoIntegrado.Services;

namespace PlanejamentoIntegrado.Configurations;

public static class Bootstrapper
{
    public static IServiceCollection UseSharedInject(
        this IServiceCollection services,
        IIntegratedPlanningConfiguration configuration
    )
    {
        services.AddDbContext<IntegratedPlanningDbContext>(options =>
            options.UseOracle(configuration.Oracle.ConnectionString)
        );

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<IStockItemService, StockItemService>();
        services.AddScoped<IScheduleService, ScheduleService>();
        services.AddScoped<IMaterialConsumedService, MaterialConsumedService>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Auth/Login";
                options.LogoutPath = "/Auth/Logout";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(120);
                options.SlidingExpiration = true;
            });

        services.AddAuthorization();

        return services;
    }
}
