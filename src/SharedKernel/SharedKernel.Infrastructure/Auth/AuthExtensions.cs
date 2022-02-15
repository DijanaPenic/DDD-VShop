using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using VShop.SharedKernel.Infrastructure.Modules;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Auth.Constants;

namespace VShop.SharedKernel.Infrastructure.Auth;

public static class AuthExtensions
{
    public static IServiceCollection AddAuth(this IServiceCollection services, params Module[] modules)
    {
        services.Configure<DataProtectionTokenProviderOptions>(options =>
        {
            // Sets the expiry to one day.
            options.TokenLifespan = TimeSpan.FromDays(1);
        });

        AuthOptions options = services.GetOptions<AuthOptions>("Auth");

        TokenValidationParameters tokenValidationParameters = new()
        {
            RequireAudience = options.RequireAudience,
            ValidIssuer = options.ValidIssuer,
            ValidIssuers = options.ValidIssuers,
            ValidateActor = options.ValidateActor,
            ValidAudience = options.ValidAudience,
            ValidAudiences = options.ValidAudiences,
            ValidateAudience = options.ValidateAudience,
            ValidateIssuer = options.ValidateIssuer,
            ValidateLifetime = options.ValidateLifetime,
            ValidateTokenReplay = options.ValidateTokenReplay,
            ValidateIssuerSigningKey = options.ValidateIssuerSigningKey,
            SaveSigninToken = options.SaveSigninToken,
            RequireExpirationTime = options.RequireExpirationTime,
            RequireSignedTokens = options.RequireSignedTokens,
            ClockSkew = TimeSpan.FromMinutes(1)
        };

        if (string.IsNullOrWhiteSpace(options.IssuerSigningKey))
            throw new ArgumentException("Missing issuer signing key.");

        byte[] rawKey = Encoding.UTF8.GetBytes(options.IssuerSigningKey);
        tokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(rawKey);
        
        if (!string.IsNullOrWhiteSpace(options.AuthenticationType))
            tokenValidationParameters.AuthenticationType = options.AuthenticationType;

        if (!string.IsNullOrWhiteSpace(options.NameClaimType))
            tokenValidationParameters.NameClaimType = options.NameClaimType;

        if (!string.IsNullOrWhiteSpace(options.RoleClaimType))
            tokenValidationParameters.RoleClaimType = options.RoleClaimType;

        void CookieAuthOptions(CookieAuthenticationOptions cookieOptions)
        {
            cookieOptions.Cookie.HttpOnly = options.Cookie.HttpOnly;
            cookieOptions.Cookie.SameSite = options.Cookie.SameSite?.ToLowerInvariant() switch
            {
                "strict" => SameSiteMode.Strict,
                "lax" => SameSiteMode.Lax,
                "none" => SameSiteMode.None,
                "unspecified" => SameSiteMode.Unspecified,
                _ => SameSiteMode.Unspecified
            };
            cookieOptions.ExpireTimeSpan = TimeSpan.FromMinutes(options.Cookie.ExpireTime);
            cookieOptions.SlidingExpiration = options.Cookie.SlidingExpiration;

            // Use 401 status code instead of Login redirect which is used by default.
            cookieOptions.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };
        }
        
        services
            .AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.Authority = options.Authority;
                o.Audience = options.Audience;
                o.MetadataAddress = options.MetadataAddress;
                o.SaveToken = options.SaveToken;
                o.RefreshOnIssuerKeyNotFound = options.RefreshOnIssuerKeyNotFound;
                o.RequireHttpsMetadata = options.RequireHttpsMetadata;
                o.IncludeErrorDetails = options.IncludeErrorDetails;
                o.TokenValidationParameters = tokenValidationParameters;
                
                if (!string.IsNullOrWhiteSpace(options.Challenge))
                    o.Challenge = options.Challenge;
                
                o.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // JWT token is sent in a different field than what is expected. This allows us to feed it in.
                        AuthenticateResult authenticateResult = context.HttpContext
                            .AuthenticateAsync(ApplicationIdentityConstants.AccessTokenScheme).GetAwaiter().GetResult();

                        if (authenticateResult?.Principal is not null)
                            context.Token = authenticateResult.Principal.FindFirstValue(ApplicationClaimTypes.Token);
                        
                        return Task.CompletedTask;
                    },
                };
            })
            .AddCookie(ApplicationIdentityConstants.AccessTokenScheme, CookieAuthOptions)
            .AddCookie(ApplicationIdentityConstants.AccountVerificationScheme, CookieAuthOptions)
            .AddCookie(IdentityConstants.TwoFactorRememberMeScheme, CookieAuthOptions)
            .AddCookie(IdentityConstants.TwoFactorUserIdScheme, CookieAuthOptions)
            .AddCookie(IdentityConstants.ExternalScheme, CookieAuthOptions);

        services.AddSingleton(options);
        services.AddSingleton(tokenValidationParameters);

        IEnumerable<string> policies = modules.SelectMany(m => m.Policies ?? Enumerable.Empty<string>());
        services.AddAuthorization(authorization =>
        {
            foreach (string policy in policies)
                authorization.AddPolicy(policy, b => b.RequireClaim("permission", policy));
        });

        return services;
    }
    
    public static IApplicationBuilder UseAuth(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.Use(async (ctx, next) =>
        {
            if (ctx.Request.Headers.ContainsKey(ApplicationIdentityConstants.AuthorizationHeader))
                ctx.Request.Headers.Remove(ApplicationIdentityConstants.AuthorizationHeader);
            
            if (ctx.Request.Cookies.Keys.Any(k => k.Contains(ApplicationIdentityConstants.AccessTokenScheme)))
            {
                AuthenticateResult authenticateResult = await ctx.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
                if (authenticateResult.Succeeded && authenticateResult.Principal is not null)
                    ctx.User = authenticateResult.Principal;
            }
        
            await next();
        });

        return app;
    }
}