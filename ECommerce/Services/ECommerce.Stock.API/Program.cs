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
    // Consumer'ý tanýtýyoruz
    x.AddConsumer<OrderCreatedEventConsumer>();

    x.AddEntityFrameworkOutbox<StockDbContext>(o =>
    {
        o.UseSqlServer();
        o.UseBusOutbox(); // Gönderimler için Outbox
    });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h => {
            h.Username("Maeglin"); // RabbitMQ panelindeki ad
            h.Password("Sjmnwt480");         // RabbitMQ panelindeki þifre
        });

        // Kuyruðu oluþtur ve Consumer ile baðla (Binding)
        cfg.ReceiveEndpoint("stock-order-created-queue", e =>
        {
            // StockDbContext üzerinden Inbox kontrolü yapýlýr
            e.UseEntityFrameworkOutbox<StockDbContext>(context);

            e.ConfigureConsumer<OrderCreatedEventConsumer>(context);
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
