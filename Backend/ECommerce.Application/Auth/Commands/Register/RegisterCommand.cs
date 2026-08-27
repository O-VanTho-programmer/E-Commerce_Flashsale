using System.Threading;
using System.Threading.Tasks;
using ECommerce.Application.Common.Interfaces.Repositories;
using ECommerce.Application.Common.Interfaces.Services;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using MediatR;

namespace ECommerce.Application.Auth.Commands.Register;

public record RegisterCommand(string Email, string Password, UserRole Role = UserRole.Customer) : IRequest<Result<string>>;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<string>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RegisterCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<string>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // 1. Check if user exists
        var existingUser = await _unitOfWork.Users.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return Result<string>.Failure("Email already in use.");
        }

        // 2. Hash Password
        string hashedPassword = _passwordHasher.HashPassword(request.Password);

        // 3. Create User
        var user = new User(request.Email, hashedPassword, request.Role);
        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // 4. Generate Token
        string token = _jwtTokenGenerator.GenerateToken(user);

        return Result<string>.Success(token);
    }
}
