using ECommerce.Order.API.Core.Application.Consumers;
using ECommerce.Order.API.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MediatR Kaydý
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<StockNotEnoughEventConsumer>();
    x.AddConsumer<StockReservedEventConsumer>();
    // MassTransit'in RabbitMQ kullanacaðýný belirtiyoruz
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h => {
            h.Username("Maeglin"); // RabbitMQ panelindeki ad
            h.Password("Sjmnwt480");         // RabbitMQ panelindeki þifre
        });
        // Endpoint yapýlandýrmasý
        cfg.ReceiveEndpoint("stock-not-enough-queue", e =>
        {
            e.ConfigureConsumer<StockNotEnoughEventConsumer>(context);
        });
        cfg.ReceiveEndpoint("stock-reserved-queue", e =>
        {
            e.ConfigureConsumer<StockReservedEventConsumer>(context);
        });
    });
});

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
