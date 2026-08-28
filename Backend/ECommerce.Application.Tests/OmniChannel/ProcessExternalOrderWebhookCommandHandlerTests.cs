using System.Threading;
using System.Threading.Tasks;
using ECommerce.Application.Common.Interfaces.Repositories;
using ECommerce.Application.OmniChannel.Commands.ProcessExternalOrderWebhook;
using ECommerce.Application.Tests.Common.Mocks;
using ECommerce.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace ECommerce.Application.Tests.OmniChannel;

public class ProcessExternalOrderWebhookCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<IExternalOrderSyncLogRepository> _mockLogRepo;
    private readonly Mock<IChannelStockAllocationRepository> _mockAllocationRepo;
    private readonly Mock<IProductVariantRepository> _mockVariantRepo;

    public ProcessExternalOrderWebhookCommandHandlerTests()
    {
        _mockUow = MockUnitOfWork.GetMockUnitOfWork();
        _mockLogRepo = new Mock<IExternalOrderSyncLogRepository>();
        _mockAllocationRepo = new Mock<IChannelStockAllocationRepository>();
        _mockVariantRepo = new Mock<IProductVariantRepository>();

        _mockUow.Setup(u => u.ExternalOrderSyncLogs).Returns(_mockLogRepo.Object);
        _mockUow.Setup(u => u.ChannelStockAllocations).Returns(_mockAllocationRepo.Object);
        _mockUow.Setup(u => u.ProductVariants).Returns(_mockVariantRepo.Object);
    }

    [Fact]
    public async Task Handle_ValidWebhook_RecordsSaleAndSucceeds()
    {
        // Arrange
        var command = new ProcessExternalOrderWebhookCommand("Shopee", "SHOPEE-123", "SKU-1", 2, "{}");
        
        // No existing log (idempotency passes)
        _mockLogRepo.Setup(l => l.GetByExternalOrderIdAsync("Shopee", "SHOPEE-123"))
                    .ReturnsAsync((ExternalOrderSyncLog?)null);

        // Variant found
        var variant = new ProductVariant(1, "SKU-1", "Red", "L", 50.0m, 100);
        variant.GetType().GetProperty("Id")?.SetValue(variant, 99);
        // We use It.IsAny since we can't easily mock the expression in FirstOrDefaultAsync
        _mockVariantRepo.Setup(v => v.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ProductVariant, bool>>>()))
                        .ReturnsAsync(variant);

        // Allocation found
        var allocation = new ChannelStockAllocation(99, "Shopee", 20);
        _mockAllocationRepo.Setup(a => a.GetAllocationAsync(99, "Shopee")).ReturnsAsync(allocation);

        var handler = new ProcessExternalOrderWebhookCommandHandler(_mockUow.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        // Assert stock was recorded
        allocation.SoldQuantity.Should().Be(2);
        allocation.GetAvailableAllocation().Should().Be(18);

        _mockLogRepo.Verify(l => l.AddAsync(It.Is<ExternalOrderSyncLog>(log => log.Status == "Processed")), Times.Once);
        _mockUow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateWebhook_IgnoresAndSucceeds()
    {
        // Arrange
        var command = new ProcessExternalOrderWebhookCommand("Shopee", "SHOPEE-123", "SKU-1", 2, "{}");
        
        // Duplicate exists!
        var existingLog = new ExternalOrderSyncLog("Shopee", "SHOPEE-123", "Processed", "{}");
        _mockLogRepo.Setup(l => l.GetByExternalOrderIdAsync("Shopee", "SHOPEE-123"))
                    .ReturnsAsync(existingLog);

        var handler = new ProcessExternalOrderWebhookCommandHandler(_mockUow.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue(); // We return success so webhook stops retrying
        
        // Assert we did NOT deduct stock or save new logs
        _mockAllocationRepo.Verify(a => a.GetAllocationAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        _mockUow.Verify(u => u.SaveChangesAsync(), Times.Never);
    }
}
