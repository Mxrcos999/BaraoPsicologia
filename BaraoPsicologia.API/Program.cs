using BaraoPsicologia.Application.Interfaces.Services;
using BaraoPsicologia.Application.Services.Email;
using BaraoPsicologia.Domain.Entities;
using BaraoPsicologia.Infra.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BaraoPsicologia.API.Extensions;

namespace BaraoPsicologia.API; 

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSwagger();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        builder.Services.AddScoped<IIdentityService, IdentityService>();

        builder.Services.AddScoped<IEmailService, EmailService>();

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options
            .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
             sqlOptions => sqlOptions.EnableRetryOnFailure(
    maxRetryCount: 5,
    maxRetryDelay: TimeSpan.FromSeconds(15),
    errorNumbersToAdd: null)));


        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
           .AddRoles<IdentityRole>()
           .AddDefaultTokenProviders()
           .AddEntityFrameworkStores<ApplicationDbContext>()
           .AddUserManager<UserManager<ApplicationUser>>()
           .AddSignInManager<SignInManager<ApplicationUser>>()
           .AddDefaultTokenProviders();

        builder.Services.AddAuthentication(builder.Configuration);

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();

        app.UseCors("AllowAll");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
