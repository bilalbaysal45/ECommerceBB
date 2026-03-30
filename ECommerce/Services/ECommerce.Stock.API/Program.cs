using ECommerce.Stock.API.Core.Application.Consumers;
using ECommerce.Stock.API.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<StockDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMassTransit(x =>
{
    //// Yeni Consumer
    x.AddConsumer<ProductCreatedEventConsumer>();

    x.AddConsumer<ReserveStockCommandConsumer>();

    x.AddEntityFrameworkOutbox<StockDbContext>(o =>
    {
        o.UseSqlServer();
        o.UseBusOutbox(); // Gönderimler için Outbox
    });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h => {
            h.Username("Maeglin");
            h.Password("Sjmnwt480");
        });

        // ProductCreatedEvent için endpoint
        cfg.ReceiveEndpoint("product-created-stock-queue", e =>
        {
            // InboxState kontrolü için StockDbContext
            e.UseEntityFrameworkOutbox<StockDbContext>(context);

            e.ConfigureConsumer<ProductCreatedEventConsumer>(context);
        });
        cfg.ReceiveEndpoint("stock-reserve-queue", e => {
            e.ConfigureConsumer<ReserveStockCommandConsumer>(context);
        });
    });
});

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
