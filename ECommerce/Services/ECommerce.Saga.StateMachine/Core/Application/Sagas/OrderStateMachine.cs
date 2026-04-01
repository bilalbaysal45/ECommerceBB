using ECommerce.Payment.API.Core.Application.Payments.Commands.ProcessPayment;
using ECommerce.Shared.Commons;
using ECommerce.Shared.Events;
using ECommerce.Shared.Events.Payments;
using ECommerce.Shared.Sagas;
using MassTransit;

namespace ECommerce.Saga.StateMachine.Core.Application.Sagas
{
    public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {
        public State Suspend { get; private set; }
        public State PaymentPending { get; private set; }
        public State Completed { get; private set; }
        public State Fail { get; private set; }

        public Event<OrderCreatedEvent> OrderCreatedEvent { get; private set; }
        public Event<StockReservedEvent> StockReservedEvent { get; private set; }
        public Event<StockNotEnoughEvent> StockNotEnoughEvent { get; private set; }
        public Event<PaymentCompletedEvent> PaymentCompletedEvent { get; private set; }
        public Event<PaymentFailedEvent> PaymentFailedEvent { get; private set; }

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
            Event(() => PaymentCompletedEvent, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => PaymentFailedEvent, x => x.CorrelateById(m => m.Message.OrderId));

            Initially(
                When(OrderCreatedEvent)
                    .Then(context => {
                        context.Saga.UserId = context.Message.UserId;
                        context.Saga.TotalPrice = context.Message.TotalPrice;
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
            .Then(context => Console.WriteLine($"Saga: Stok rezerve edildi, ödeme emri gönderiliyor. OrderId: {context.Saga.CorrelationId}"))
            .Send(new Uri("queue:payment-process-queue"), context => new ProcessPaymentCommand
            {
                OrderId = context.Saga.CorrelationId,
                TotalPrice = context.Saga.TotalPrice // OrderState içinde bu verinin olduğundan emin ol
            })
            .TransitionTo(PaymentPending), // Ödeme beklemeye geç
                When(StockNotEnoughEvent)
                    .TransitionTo(Fail)
                    .Then(context => Console.WriteLine($"Saga: Sipariş {context.Saga.CorrelationId} stok nedeniyle başarısız."))
            );
            // 4. ÖDEME SONUCUNU KARŞILAMA
            During(PaymentPending,
                When(PaymentCompletedEvent)
                    .Then(context => Console.WriteLine($"Saga: Ödeme başarılı. Sipariş tamamlandı. OrderId: {context.Saga.CorrelationId}"))
                    .TransitionTo(Completed),

                When(PaymentFailedEvent)
                    .Then(context => Console.WriteLine($"Saga: Ödeme başarısız! Hata: {context.Message.Message}"))
                    .TransitionTo(Fail)
            // İPUCU: Burada ileride "Compensating Transaction" (Stok iadesi) ekleyeceğiz.
            );
        }
    }
}
