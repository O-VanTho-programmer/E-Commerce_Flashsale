using System.Threading;
using System.Threading.Tasks;
using ECommerce.Application.Common.Interfaces.Repositories;
using ECommerce.Application.Common.Models;
using ECommerce.Domain.Entities;
using MediatR;

namespace ECommerce.Application.Category.Commands.CreateCategory;

public record CreateCategoryCommand(string Name, string Slug, int? ParentCategoryId) : IRequest<Result<int>>;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (request.ParentCategoryId.HasValue)
        {
            var parentCategory = await _unitOfWork.Categories.GetByIdAsync(request.ParentCategoryId.Value);
            if (parentCategory == null)
            {
                return Result<int>.Failure("Parent Category does not exist.");
            }
        }

        var newCategory = new ECommerce.Domain.Entities.Category(request.Name, request.Slug, request.ParentCategoryId);
        
        await _unitOfWork.Categories.AddAsync(newCategory);
        await _unitOfWork.SaveChangesAsync();

        return Result<int>.Success(newCategory.Id);
    }
}
