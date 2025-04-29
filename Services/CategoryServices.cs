using Microsoft.EntityFrameworkCore;
using MidAssignment.Domain;
using MidAssignment.DTOs;
using MidAssignment.DTOs.Category;
using MidAssignment.Repositories;
using MidAssignment.Services.Interfaces;
using System.Linq.Expressions;

namespace MidAssignment.Services
{
    public class CategoryServices(IRepository<Category> cateRepo) : ICategoryServices
    {
        private readonly IRepository<Category> _cateRepo = cateRepo;
        public async Task<ApplicationResponse> CreateNewCategory(CreateCategoryDto newCategoryDto)
        {
            var categoryExists = _cateRepo.GetQueryable(c => c.Name == newCategoryDto.Name);
            if (categoryExists.ToList().Count > 0)
            {
                return new ErrorApplicationResponse(StatusCodes.Status400BadRequest, [$"Category name {newCategoryDto.Name} is already exists"]);
            }
            var newCategory = new Category
            {
                Name = newCategoryDto.Name,
            };

            await _cateRepo.AddAsync(newCategory);

            var result = await _cateRepo.SaveChangesAsync();

            if (result)
            {
                var categoryResponse = new CategoryDto(newCategory.Id, newCategory.Name);
                return new SuccessApplicationResponse<CategoryDto>(StatusCodes.Status201Created, categoryResponse);
            }
            else
            {
                return new ErrorApplicationResponse(StatusCodes.Status500InternalServerError, ["Please try again later"]);
            }
        }



        public async Task<ApplicationResponse> GetCategories(int currentPage, int limit, string? name)
        {
            if (currentPage <= 0) currentPage = 1;
            if (limit <= 0) limit = 5;

            // Build predicate
            Expression<Func<Category, bool>> predicate = c => string.IsNullOrEmpty(name) || c.Name.Contains(name);

            // Fetch filtered categories
            IQueryable<Category> query = _cateRepo.GetQueryable(predicate);

            int totalRecords = await query.AsNoTracking().CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalRecords / limit);

            if (currentPage > totalPages && totalPages > 0)
            {
                return new ErrorApplicationResponse(StatusCodes.Status400BadRequest, ["This page is out of scope"]);
            }

            var paginatedCategories = await query
                .Skip((currentPage - 1) * limit)
                .Take(limit)
                .Select(c => new CategoryDto(c.Id, c.Name))
                .AsNoTracking()
                .ToListAsync();

            var result = new PaginateDto<CategoryDto>(
                paginatedCategories,
                totalPages,
                currentPage
               );

            return new SuccessApplicationResponse<PaginateDto<CategoryDto>>(StatusCodes.Status200OK, result);
        }


        public async Task<ApplicationResponse> GetCategoryById(Guid id)
        {
            Category? matchCategory = await _cateRepo.GetByIdAsync(id);
            if (matchCategory == null)
            {
                return new ErrorApplicationResponse(StatusCodes.Status404NotFound, ["Category not found"]);
            }
            var categoryResponse = new CategoryDto(matchCategory.Id, matchCategory.Name);
            return new SuccessApplicationResponse<CategoryDto>(StatusCodes.Status200OK, categoryResponse);
        }
        public async Task<ApplicationResponse> UpdateCategoryById(Guid updateId, CreateCategoryDto updateCategoryDto)
        {
            Category? existCategory = await _cateRepo.GetByIdAsync(updateId);
            if (existCategory == null)
            {
                return new ErrorApplicationResponse(StatusCodes.Status404NotFound, [$"Category id {updateId} is not found"]);
            }

            var categoryExists = _cateRepo.GetQueryable(c => c.Name == updateCategoryDto.Name);
            if (await categoryExists.AnyAsync())
            {
                return new ErrorApplicationResponse(StatusCodes.Status400BadRequest, [$"New category name {updateCategoryDto.Name} is already exists"]);
            }

            // Update fields
            existCategory.Name = updateCategoryDto.Name;

            // Save changes
            _cateRepo.Update(existCategory);
            var result = await _cateRepo.SaveChangesAsync();

            if (result)
            {
                var categoryResponse = new CategoryDto(existCategory.Id, existCategory.Name);
                return new SuccessApplicationResponse<CategoryDto>(StatusCodes.Status200OK, categoryResponse);
            }
            else
            {
                return new ErrorApplicationResponse(StatusCodes.Status500InternalServerError, ["Please try again later"]);
            }
        }

        public async Task<ApplicationResponse> DeleteCategoryById(Guid deleteId)
        {
            // Find the category by ID
            Category? existCategory = await _cateRepo.GetByIdAsync(deleteId);
            if (existCategory == null)
            {
                return new ErrorApplicationResponse(StatusCodes.Status404NotFound, [$"Category id {deleteId} is not found"]);
            }

            // Delete the category
            await _cateRepo.Delete(deleteId);
            var result = await _cateRepo.SaveChangesAsync();

            if (result)
            {
                var categoryResponse = new CategoryDto(existCategory.Id, existCategory.Name);
                return new SuccessApplicationResponse<CategoryDto>(StatusCodes.Status200OK, categoryResponse);
            }
            else
            {
                return new ErrorApplicationResponse(StatusCodes.Status500InternalServerError, ["Please try again later"]);
            }
        }
    }
}
