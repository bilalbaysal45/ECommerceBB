using ECommerce.Product.API.Core.Application.Mapping;
using ECommerce.Product.API.Core.Application.Pipelines;
using ECommerce.Product.API.Infrastructure.Middlewares;
using ECommerce.Product.API.Infrastructure.Persistence;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
// MediatR'ý sisteme kaydediyoruz. 
// 'typeof(Program).Assembly' ifadesi, uygulamadaki tüm Handler sýnýflarýný otomatik taramasýný saðlar.
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    // Pipeline Behavior kaydý
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "ECommerce Product API", Version = "v1" });

    // 1. Güvenlik Tanýmý (Security Definition)
    var securityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter JWT Bearer token **only**",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer", // "bearer" küçük harfle olmalý
        BearerFormat = "JWT",
        Reference = new Microsoft.OpenApi.Models.OpenApiReference
        {
            Id = "Bearer", // Bu ID, alttaki gereksinimle ayný olmalý
            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme // HATA BURADAYDI: ReferenceType kullanýyoruz
        }
    };

    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);

    // 2. Güvenlik Gereksinimi (Security Requirement)
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        { securityScheme, new string[] { } }
    });
});

// JWT AUTH
#region JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        ClockSkew = TimeSpan.Zero // Zaman farklarýný sýfýrla
    };
});
#endregion

var app = builder.Build();

// Hatalarý en üstte yakalamasý için en baþa ekliyoruz
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Kimsin?
app.UseAuthorization();  // Yetkin var mý?

app.MapControllers();

app.Run();
