using BaraoPsicologia.API.Extensions;
using BaraoPsicologia.Application.Dto.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BaraoPsicologia.API.Extensions;

public static class AuthenticationSetup
{
    public static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var JwtAppSettings = configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>();

        var securityKey = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(
                configuration.GetSection("JwtOptions:SecurityKey").Value
            )
        );

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = JwtAppSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = JwtAppSettings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = securityKey,
            RequireExpirationTime = true,
            ValidateLifetime = true
        };

        //Configuração de geração de Token
        services.Configure<JwtOptions>(options =>
        {
            options.Issuer = JwtAppSettings.Issuer;
            options.Audience = JwtAppSettings.Audience;
            options.SigningCredentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256
            ); ;
            options.AccessTokenExpiration = JwtAppSettings.AccessTokenExpiration;
        });

        //Requisitos de geração de senha senha
        services.Configure<IdentityOptions>(options =>
        {
            options.SignIn.RequireConfirmedEmail = true;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 4;
            options.Password.RequiredUniqueChars = 0;
        });

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = tokenValidationParameters;
        });

        services.AddAuthorization();
    }
}
