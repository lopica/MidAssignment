using MidAssignment.DTOs;
using MidAssignment.DTOs.Category;

namespace MidAssignment.Services.Interfaces
{
    public interface ICategoryServices
    {
        Task<ApplicationResponse> CreateNewCategory(CreateCategoryDto newCategoryDto);
        Task<ApplicationResponse> GetCategories(int currentPage, int limit, string? name = null);
        Task<ApplicationResponse> GetCategoryById(Guid id);
        Task<ApplicationResponse> UpdateCategoryById(Guid updateId, CreateCategoryDto updateCategoryDto);
        Task<ApplicationResponse> DeleteCategoryById(Guid deleteId);
    }
}
