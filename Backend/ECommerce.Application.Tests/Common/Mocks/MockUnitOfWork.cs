using ECommerce.Application.Common.Interfaces.Repositories;
using Moq;

namespace ECommerce.Application.Tests.Common.Mocks;

public static class MockUnitOfWork
{
    public static Mock<IUnitOfWork> GetMockUnitOfWork()
    {
        var mockUow = new Mock<IUnitOfWork>();
        
        // Setup standard successful SaveChangesAsync
        mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        return mockUow;
    }
}
