using ECommerce.Payment.API.Core.Application.Consumers;
using ECommerce.Payment.API.Infrastructure.Persistence;
using MassTransit;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 1. MediatR Kaydý
// Mevcut assembly içindeki tüm Handler'larý (ProcessPaymentHandler gibi) otomatik bulur.
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

// 2. MassTransit & RabbitMQ Kaydý
builder.Services.AddMassTransit(x =>
{
    // Consumer'ý MassTransit'e tanýtýyoruz
    x.AddConsumer<ProcessPaymentCommandConsumer>();

    x.AddEntityFrameworkOutbox<PaymentDbContext>(o =>
    {
        o.UseSqlServer();
        o.UseBusOutbox();
    });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", host =>
        {
            host.Username("Maeglin");
            host.Password("Sjmnwt480");
        });

        // Saga'dan gelecek olan 'payment-process-queue' kuyruðunu dinliyoruz
        cfg.ReceiveEndpoint("payment-process-queue", e =>
        {
            e.ConfigureConsumer<ProcessPaymentCommandConsumer>(context);
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

app.Run();