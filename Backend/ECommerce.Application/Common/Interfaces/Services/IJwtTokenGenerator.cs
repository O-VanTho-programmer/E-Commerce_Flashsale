using ECommerce.Domain.Entities;

namespace ECommerce.Application.Common.Interfaces.Services;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
