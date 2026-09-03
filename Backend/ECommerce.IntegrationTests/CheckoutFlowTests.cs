using System.Threading.Tasks;
using ECommerce.Application.Orders.Commands.PlaceOrder;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using ECommerce.Infrastructure.Data;
using FluentAssertions;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ECommerce.IntegrationTests;

public class CheckoutFlowTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly IntegrationTestWebAppFactory _factory;

    public CheckoutFlowTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PlaceOrderCommand_ShouldCreateOrder_ClearCart_And_DeductStock()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var user = new User("testuser@example.com", "mockhash", UserRole.Customer);
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var category = new Category("Shoes", "shoes");
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var product = new Product(category.Id, "Sneakers", "Nice shoes");
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var variant = new ProductVariant(product.Id, "SNK-42", "White", "42", 100, 10);
        db.ProductVariants.Add(variant);
        await db.SaveChangesAsync();

        var cart = new Cart(user.Id);
        db.Carts.Add(cart);
        await db.SaveChangesAsync();

        var cartItem = new CartItem(cart.Id, variant.Id, 2, false);
        db.Set<CartItem>().Add(cartItem);
        await db.SaveChangesAsync();

        var mediator = scope.ServiceProvider.GetRequiredService<MediatR.IMediator>();
        var command = new PlaceOrderCommand(user.Id);

        // Act
        var orderResult = await mediator.Send(command);

        // Assert 1: Order created
        orderResult.IsSuccess.Should().BeTrue();
        var orderId = orderResult.Data;
        orderId.Should().BePositive();
        
        var order = await db.Orders.FindAsync(orderId);
        order.Should().NotBeNull();
        order!.Status.Should().Be(OrderStatus.Pending);
        order.TotalAmount.Should().Be(200);

        // Assert 2: Cart is cleared
        var cartAfter = await db.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.UserId == user.Id);
        cartAfter!.CartItems.Should().BeEmpty();

        // Assert 3: MassTransit Event Published & Consumed
        var harness = scope.ServiceProvider.GetRequiredService<ITestHarness>();
        var published = await harness.Published.Any<ECommerce.Application.Common.Events.OrderPlacedEvent>();
        published.Should().BeTrue("Because the event should be published via Outbox and MassTransit.");

        var consumed = await harness.Consumed.Any<ECommerce.Application.Common.Events.OrderPlacedEvent>();
        consumed.Should().BeTrue("Because DeductStockOnOrderPlacedConsumer should consume it.");

        // Assert 4: Inventory Deducted
        await db.Entry(variant).ReloadAsync();
        variant.StockQuantity.Should().Be(8, "Because 10 - 2 = 8.");
    }
}
