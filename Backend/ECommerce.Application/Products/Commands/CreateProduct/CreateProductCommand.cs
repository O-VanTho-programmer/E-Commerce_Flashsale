using System.Threading;
using System.Threading.Tasks;
using ECommerce.Application.Common.Interfaces.Repositories;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Entities;
using MediatR;

namespace ECommerce.Application.Products.Commands.CreateProduct;

public record CreateProductCommand(string Name, string Description, int CategoryId) : IRequest<Result<int>>;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId);
        if (category == null)
            return Result<int>.Failure("Category does not exist.");

        var newProduct = new Product(request.CategoryId, request.Name, request.Description);
        
        await _unitOfWork.Products.AddAsync(newProduct);
        await _unitOfWork.SaveChangesAsync();

        // Return the new Product's ID
        return Result<int>.Success(newProduct.Id);
    }
}
