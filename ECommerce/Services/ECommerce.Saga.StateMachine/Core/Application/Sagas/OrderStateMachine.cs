using ECommerce.Shared.Commons;
using ECommerce.Shared.Events;
using ECommerce.Shared.Sagas;
using MassTransit;

namespace ECommerce.Saga.StateMachine.Core.Application.Sagas
{
    public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {
        public State Suspend { get; private set; }
        public State Completed { get; private set; }
        public State Fail { get; private set; }

        public Event<OrderCreatedEvent> OrderCreatedEvent { get; private set; }
        public Event<StockReservedEvent> StockReservedEvent { get; private set; }
        public Event<StockNotEnoughEvent> StockNotEnoughEvent { get; private set; }

        public OrderStateMachine()
        {
            // Bu satırları constructor'ın en başına ekle
            State(() => Suspend);
            State(() => Completed);
            State(() => Fail);

            InstanceState(x => x.CurrentState);
            // Mesajlardaki OrderId ile Saga'daki CorrelationId'yi eşleştiriyoruz
            Event(() => OrderCreatedEvent, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => StockReservedEvent, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => StockNotEnoughEvent, x => x.CorrelateById(m => m.Message.OrderId));

            Initially(
                When(OrderCreatedEvent)
                    .Then(context => {
                        context.Saga.UserId = context.Message.UserId;
                    })
                    .Send(new Uri($"queue:stock-reserve-queue"), context => new ReserveStockCommand
                    {
                        OrderId = context.Message.OrderId,
                        OrderItems = context.Message.Items.Select(x => new OrderItemMessage
                        {
                            ProductId = x.ProductId,
                            Quantity = x.Quantity
                        }).ToList()
                    })
                    .TransitionTo(Suspend)
            );

            During(Suspend,
                When(StockReservedEvent)
                    .TransitionTo(Completed)
                    .Then(context => Console.WriteLine($"Saga: Sipariş {context.Saga.CorrelationId} başarıyla tamamlandı.")),

                When(StockNotEnoughEvent)
                    .TransitionTo(Fail)
                    .Then(context => Console.WriteLine($"Saga: Sipariş {context.Saga.CorrelationId} stok nedeniyle başarısız."))
            );
        }
    }
}
