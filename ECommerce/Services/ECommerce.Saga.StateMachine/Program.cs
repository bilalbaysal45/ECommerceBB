using ECommerce.Saga.StateMachine.Core.Application.Sagas;
using ECommerce.Saga.StateMachine.Core.Domain.Entities;
using ECommerce.Saga.StateMachine.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    x.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .EntityFrameworkRepository(r =>
        {
            r.ExistingDbContext<SagaDbContext>();
            r.UseSqlServer();
            // Saga yüklendiðinde OrderItems tablosunu da beraberinde getir
            r.CustomizeQuery(query => query.Include(s => s.OrderItems));
        });
    x.AddEntityFrameworkOutbox<SagaDbContext>(o =>
    {
        // Mesajlarý veritabanýnda sakla
        o.UseSqlServer();
        o.UseBusOutbox();
    });
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("Maeglin");
            h.Password("Sjmnwt480");
        });
        cfg.ReceiveEndpoint("order-state-queue", e =>
        {
            e.UseEntityFrameworkOutbox<SagaDbContext>(context);
            // Bu satýr Saga'yý ve onun dinlediði eventleri (StockReservedEvent vb.) bu kuyruða baðlar
            e.ConfigureSaga<OrderState>(context);
        });
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddDbContext<SagaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();