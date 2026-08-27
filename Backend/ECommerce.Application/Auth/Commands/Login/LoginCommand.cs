using System.Threading;
using System.Threading.Tasks;
using ECommerce.Application.Common.Interfaces.Repositories;
using ECommerce.Application.Common.Interfaces.Services;
using ECommerce.Application.Common.Models;
using MediatR;

namespace ECommerce.Application.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<Result<string>>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<string>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch user
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
        if (user == null)
        {
            return Result<string>.Failure("Invalid email or password.");
        }

        // 2. Verify Password
        bool isPasswordValid = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            return Result<string>.Failure("Invalid email or password.");
        }

        // 3. Generate Token
        string token = _jwtTokenGenerator.GenerateToken(user);

        return Result<string>.Success(token);
    }
}
