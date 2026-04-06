using ECommerce.Gateway.API;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "GatewayKey";
    options.DefaultChallengeScheme = "GatewayKey";
})
    .AddJwtBearer("GatewayKey", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            //ValidAudience = builder.Configuration["JwtSettings:Audience"],
            ValidAudience = "ECommerce.Clients",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"])),
            RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
            NameClaimType = "userId"
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                // Hata mesajýný buraya yazdýrýyoruz
                Console.WriteLine("Auth Failed: " + context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Auth Success!");
                return Task.CompletedTask;
            }
        };

    });
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

//builder.Services.AddOcelot(builder.Configuration);
// 1. HttpContext'e eriþim için gerekli servisi ekle (AddOcelot'tan önce ekle)
builder.Services.AddHttpContextAccessor();

// 2. Ocelot ve Handler kaydý
builder.Services.AddOcelot()
    .AddDelegatingHandler<ClaimToHeaderHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

await app.UseOcelot();
app.Run();