﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.IdentityModel.Tokens;
using ShoeGrabCommonModels;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ShoeGrabAdminService.Extensions;

public static class BuilderExtension
{
    public static void AddGrpcAndClients(this IServiceCollection services, IConfiguration configuration)
    {
        var productService = services.AddGrpcClient<ProductManagement.ProductManagementClient>(options =>
        {
            options.Address = new Uri(Environment.GetEnvironmentVariable("PRODUCT_MANAGEMENT_CONNECTION_STRING"));
        });

        var orderService = services.AddGrpcClient<OrderManagement.OrderManagementClient>(options =>
        {
            options.Address = new Uri(Environment.GetEnvironmentVariable("ORDER_MANAGEMENT_CONNECTION_STRING"));
        });
    }
    public static void AddJWTAuthenticationAndAuthorization(this WebApplicationBuilder builder)
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        var secretKey = Encoding.ASCII.GetBytes(jwtSettings["Secret"]);

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireRole(UserRole.Admin));

            options.AddPolicy("UserOnly", policy =>
                policy.RequireRole(UserRole.User));
        });
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.IncludeErrorDetails = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                ValidAudience = builder.Configuration["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"])),
                RoleClaimType = ClaimTypes.Role
            };
        });
        builder.Services.AddAuthorization();
    }

    public static void SetupKestrel(this WebApplicationBuilder builder)
    {
        builder.WebHost.UseKestrel(options =>
        {
            options.Configure();
        });

        builder.WebHost.ConfigureKestrel((context, options) =>
        {
            var kestrelSection = context.Configuration.GetSection("Kestrel:Endpoints");

            var restApiEndpoint = kestrelSection.GetSection("RestApi");
            if (restApiEndpoint.Exists())
            {
                var restApiUrl = new Uri(Environment.GetEnvironmentVariable("ADMIN_REST_URI"));
                options.Listen(IPAddress.Parse(restApiUrl.Host), restApiUrl.Port, listenOptions =>
                {
                    listenOptions.Protocols = Enum.Parse<HttpProtocols>(restApiEndpoint["Protocols"]);
                });
            }
        });
    }
}
