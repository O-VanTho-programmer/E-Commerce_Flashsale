using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ECommerce.Application.Common.Interfaces.Repositories;
using ECommerce.Application.Common.Interfaces.Services;
using ECommerce.Application.Orders.Commands.PlaceOrder;
using ECommerce.Application.Tests.Common.Mocks;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using FluentAssertions;
using Moq;
using Xunit;

namespace ECommerce.Application.Tests.Orders;

public class PlaceOrderCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<ICartRepository> _mockCartRepo;
    private readonly Mock<IStockReservationRepository> _mockReservationRepo;
    private readonly Mock<IOrderRepository> _mockOrderRepo;
    private readonly Mock<ICartItemRepository> _mockCartItemRepo;
    private readonly Mock<IOrderItemRepository> _mockOrderItemRepo;
    private readonly Mock<IInventoryService> _mockInventoryService;

    public PlaceOrderCommandHandlerTests()
    {
        _mockUow = MockUnitOfWork.GetMockUnitOfWork();
        _mockCartRepo = new Mock<ICartRepository>();
        _mockReservationRepo = new Mock<IStockReservationRepository>();
        _mockOrderRepo = new Mock<IOrderRepository>();
        _mockCartItemRepo = new Mock<ICartItemRepository>();
        _mockOrderItemRepo = new Mock<IOrderItemRepository>();
        _mockInventoryService = new Mock<IInventoryService>();

        _mockUow.Setup(u => u.Carts).Returns(_mockCartRepo.Object);
        _mockUow.Setup(u => u.StockReservations).Returns(_mockReservationRepo.Object);
        _mockUow.Setup(u => u.Orders).Returns(_mockOrderRepo.Object);
        _mockUow.Setup(u => u.CartItems).Returns(_mockCartItemRepo.Object);
        _mockUow.Setup(u => u.OrderItems).Returns(_mockOrderItemRepo.Object);
    }

    [Fact]
    public async Task Handle_GivenValidCartAndReservations_PlacesOrderSuccessfully()
    {
        // Arrange
        var command = new PlaceOrderCommand(1);
        
        var cart = new ECommerce.Domain.Entities.Cart(1);
        cart.GetType().GetProperty("Id")?.SetValue(cart, 100); 
        
        var variant = new ProductVariant(1, "SKU-1", "Red", "L", 50.0m, 10);
        variant.GetType().GetProperty("Id")?.SetValue(variant, 200);

        var cartItem = new CartItem(100, 200, 2, false);
        cartItem.GetType().GetProperty("Id")?.SetValue(cartItem, 300);
        cartItem.GetType().GetProperty("ProductVariant")?.SetValue(cartItem, variant);
        
        // Use reflection to add item since it's an internal collection
        var itemsField = typeof(ECommerce.Domain.Entities.Cart).GetProperty("CartItems");
        itemsField?.SetValue(cart, new List<CartItem> { cartItem });
        
        _mockCartRepo.Setup(c => c.GetByUserIdWithItemsAsync(1)).ReturnsAsync(cart);
        _mockInventoryService.Setup(i => i.IsStockAvailableAsync(200, 2, false)).ReturnsAsync(true);

        var reservation = new StockReservation(300, 200, 2, DateTime.UtcNow.AddMinutes(15));
        reservation.GetType().GetProperty("Status")?.SetValue(reservation, StockReservationStatus.Reserved);
        
        _mockReservationRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<StockReservation, bool>>>())).ReturnsAsync(reservation);

        var handler = new PlaceOrderCommandHandler(_mockUow.Object, _mockInventoryService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        // Assert Reservation was confirmed
        reservation.Status.Should().Be(StockReservationStatus.Confirmed);
        
        _mockOrderRepo.Verify(o => o.AddAsync(It.IsAny<Order>()), Times.Once);
        _mockUow.Verify(u => u.SaveChangesAsync(), Times.Exactly(2));
        _mockCartItemRepo.Verify(c => c.Delete(cartItem), Times.Once);
    }

    [Fact]
    public async Task Handle_GivenEmptyCart_ReturnsFailure()
    {
        // Arrange
        var command = new PlaceOrderCommand(1);
        var cart = new ECommerce.Domain.Entities.Cart(1); // Empty cart
        
        _mockCartRepo.Setup(c => c.GetByUserIdWithItemsAsync(1)).ReturnsAsync(cart);

        var handler = new PlaceOrderCommandHandler(_mockUow.Object, _mockInventoryService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Your cart is empty.");
        _mockOrderRepo.Verify(o => o.AddAsync(It.IsAny<Order>()), Times.Never);
    }
}
