using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.Sagas
{
    public class OrderState : SagaStateMachineInstance, ISagaVersion

    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }

        // Süreç takibi için ek veriler
        public string UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public int Version { get; set; }
    }
}
