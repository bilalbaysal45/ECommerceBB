using ECommerce.Order.API.Core.Application.Orders.Commands.CreateOrder;
using FluentValidation;

namespace ECommerce.Order.API.Core.Application.Orders.Validators
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            // Kullanıcı ID boş olamaz
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("Kullanıcı bilgisi boş olamaz.");

            // En az bir ürün olmalı
            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Siparişte en az bir ürün bulunmalıdır.")
                .Must(x => x.Count > 0).WithMessage("Ürün listesi boş olamaz.");

            // Liste içindeki her bir ürün (OrderItem) için kurallar
            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.ProductId)
                    .NotEmpty().WithMessage("Ürün ID boş olamaz.");

                item.RuleFor(i => i.Quantity)
                    .GreaterThan(0).WithMessage("Ürün adedi 0'dan büyük olmalıdır.");

                item.RuleFor(i => i.Price)
                    .GreaterThan(0).WithMessage("Ürün fiyatı 0'dan büyük olmalıdır.");
            });
        }
    }
}
