using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

using VShop.Modules.Identity.Infrastructure.Services;
using VShop.Modules.Identity.Infrastructure.DAL.Entities;
using VShop.Modules.Identity.Infrastructure.DAL.Stores;
using VShop.Modules.Identity.Infrastructure.DAL.Stores.Contracts;

namespace VShop.Modules.Identity.Infrastructure.Configuration.Extensions;

internal static class IdentityExtensions
{
    public static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services.Configure<DataProtectionTokenProviderOptions>(options =>
        {
            // Sets the expiry to one day.
            options.TokenLifespan = TimeSpan.FromDays(1);
        });
        
        services.AddIdentityCore<User>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;

            options.Lockout.AllowedForNewUsers = true;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
            options.Lockout.MaxFailedAccessAttempts = 3;

            options.User.RequireUniqueEmail = true;

            // Changing these settings will require adjustments in SignInManager and SignIn command handlers.
            options.SignIn.RequireConfirmedEmail = true;
            options.SignIn.RequireConfirmedPhoneNumber = true;
        })
        .AddRoles<Role>()
        .AddUserManager<ApplicationUserManager>()
        .AddRoleManager<ApplicationRoleManager>()
        .AddDefaultTokenProviders();

        services.AddScoped<ApplicationSignInManager>();
        services.AddScoped<ApplicationAuthManager>();

        services.AddScoped<IUserStore<User>, ApplicationUserStore>();
        services.AddScoped<IRoleStore<Role>, ApplicationRoleStore>();
        services.AddScoped<IApplicationClientStore, ApplicationAuthStore>();
        services.AddScoped<IApplicationUserRefreshTokenStore, ApplicationAuthStore>();
        services.AddTransient<IAuthService, AuthService>();
        
        return services;
    }
}