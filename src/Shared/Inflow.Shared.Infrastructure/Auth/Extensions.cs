using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inflow.Shared.Abstractions.Auth;
using Inflow.Shared.Abstractions.Modules;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Inflow.Shared.Infrastructure.Auth;

public static class Extensions
{
    private const string AccessTokenCookieName = "__access-token";
    private const string AuthorizationHeader = "authorization";
    private const string Permissions = "permissions";

    public static IServiceCollection AddAuth(this IServiceCollection services, IList<IModule> modules = null,
        Action<JwtBearerOptions> optionsFactory = null)
    {
        var authOptions = services.GetOptions<AuthOptions>("auth");
        services.AddSingleton<IAuthManager, AuthManager>();

        if (authOptions.AuthenticationDisabled)
        {
            services.AddSingleton<IPolicyEvaluator, DisabledAuthenticationPolicyEvaluator>();
        }
        
        services.AddSingleton(new CookieOptions()
        {
            HttpOnly = authOptions.Cookie.HttpOnly,
            Secure = authOptions.Cookie.Secure,
            SameSite = authOptions.Cookie.SameSite?.ToLowerInvariant() switch
            {
                "strict" => SameSiteMode.Strict,
                "lax" => SameSiteMode.Lax,
                "none" => SameSiteMode.None,
                "unspecified" => SameSiteMode.Unspecified,
                _ => SameSiteMode.Unspecified
            }
        });
        
        var tokenValidationParameters = new TokenValidationParameters
        {
            RequireAudience = authOptions.RequireAudience,
            ValidIssuer = authOptions.ValidIssuer,
            ValidIssuers = authOptions.ValidIssuers,
            ValidateActor = authOptions.ValidateActor,
            ValidAudience = authOptions.ValidAudience,
            ValidAudiences = authOptions.ValidAudiences,
            ValidateAudience = authOptions.ValidateAudience,
            ValidateIssuer = authOptions.ValidateIssuer,
            ValidateLifetime = authOptions.ValidateLifetime,
            ValidateTokenReplay = authOptions.ValidateTokenReplay,
            ValidateIssuerSigningKey = authOptions.ValidateIssuerSigningKey,
            SaveSigninToken = authOptions.SaveSigninToken,
            RequireExpirationTime = authOptions.RequireExpirationTime,
            RequireSignedTokens = authOptions.RequireSignedTokens,
            ClockSkew = TimeSpan.Zero
        };
        
        if (string.IsNullOrWhiteSpace(authOptions.IssuerSigningKey))
        {
            throw new ArgumentException("Missing issuer signing key.", nameof(authOptions.IssuerSigningKey));
        }
        
        if (!string.IsNullOrWhiteSpace(authOptions.AuthenticationType))
        {
            tokenValidationParameters.AuthenticationType = authOptions.AuthenticationType;
        }

        var rawKey = Encoding.UTF8.GetBytes(authOptions.IssuerSigningKey);
        tokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(rawKey);
        
        if (!string.IsNullOrWhiteSpace(authOptions.NameClaimType))
        {
            tokenValidationParameters.NameClaimType = authOptions.NameClaimType;
        }

        if (!string.IsNullOrWhiteSpace(authOptions.RoleClaimType))
        {
            tokenValidationParameters.RoleClaimType = authOptions.RoleClaimType;
        }

        services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.Authority = authOptions.Authority;
                o.Audience = authOptions.Audience;
                o.MetadataAddress = authOptions.MetadataAddress;
                o.SaveToken = authOptions.SaveToken;
                o.RefreshOnIssuerKeyNotFound = authOptions.RefreshOnIssuerKeyNotFound;
                o.RequireHttpsMetadata = authOptions.RequireHttpsMetadata;
                o.IncludeErrorDetails = authOptions.IncludeErrorDetails;
                o.TokenValidationParameters = tokenValidationParameters;
                if (!string.IsNullOrWhiteSpace(authOptions.Challenge))
                {
                    o.Challenge = authOptions.Challenge;
                }

                o.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = ctx =>
                    {
                        if (ctx.Request.Cookies.TryGetValue(AccessTokenCookieName, out var token))
                        {
                            ctx.Token = token;
                        }

                        return Task.CompletedTask;
                    }
                };

                optionsFactory?.Invoke(o);
            });

        services.AddSingleton(authOptions);
        services.AddSingleton(authOptions.Cookie);
        services.AddSingleton(tokenValidationParameters);

        var policies = modules?.SelectMany(x => x.Policies ?? Enumerable.Empty<string>())
                       ?? Enumerable.Empty<string>();

        services.AddAuthorization(auth =>
        {
            foreach (var policy in policies)
            {
                auth.AddPolicy(policy, x => x.RequireClaim(Permissions, policy));
            }
        });

        return services;
    }

    public static IApplicationBuilder UseAuth(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        
        app.Use(async (ctx, next) =>
        {
            if (ctx.Request.Headers.ContainsKey(AuthorizationHeader))
            {
                ctx.Request.Headers.Remove(AuthorizationHeader);
            }

            if (ctx.Request.Cookies.ContainsKey(AccessTokenCookieName))
            {
                var authResult = await ctx.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
                if (authResult.Succeeded && authResult.Principal is not null)
                {
                    ctx.User = authResult.Principal;
                }
            }

            await next();
        });

        return app;
    }
}