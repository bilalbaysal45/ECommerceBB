
using ECommerce.Order.API.Core.Application.Orders.Commands.CreateOrder;
using ECommerce.Order.API.Core.Application.Orders.Validators;
using ECommerce.Order.API.Models.Dtos;
using ECommerce.Shared.Commons;
using FluentValidation.TestHelper;

namespace ECommerce.Product.UnitTests.Application.Orders
{
    public class CreateOrderCommandValidatorTests
    {
        private readonly CreateOrderCommandValidator _validator;

        public CreateOrderCommandValidatorTests()
        {
            _validator = new CreateOrderCommandValidator();
        }
        [Fact]
        public void Should_Have_Error_When_UserId_Is_Empty()
        {
            var orderItemCreateDtoList = new List<OrderItemCreateDto>{ new OrderItemCreateDto {Price = 1, ProductId = new Guid(),Quantity=2 } };
            var command = new CreateOrderCommand(UserId: "",Items: orderItemCreateDtoList);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.UserId);
        }

        [Fact]
        public void Should_Have_Error_When_Items_Are_Empty()
        {
            var command = new CreateOrderCommand(UserId: "UserId123", Items: new List<OrderItemCreateDto>());
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Items);
        }

        [Fact]
        public void Should_Have_Error_When_Price_Is_Negative()
        {
            var orderItemCreateDtoList = new List<OrderItemCreateDto> { new OrderItemCreateDto { Price = -1, ProductId = new Guid(), Quantity = 2 } };
            var command = new CreateOrderCommand(UserId: "UserId123", Items: orderItemCreateDtoList);
            var result = _validator.TestValidate(command);

            // ChildRules içindeki hatayı yakalamak için:
            result.ShouldHaveValidationErrors();
        }

        [Fact]
        public void Should_Not_Have_Error_When_Command_Is_Valid()
        {
            var productId = Guid.NewGuid();
            var orderItemCreateDtoList = new List<OrderItemCreateDto> { new OrderItemCreateDto { Price = 50, ProductId = productId, Quantity = 2 } };
            var command = new CreateOrderCommand(UserId: "UserId123", Items: orderItemCreateDtoList);
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
