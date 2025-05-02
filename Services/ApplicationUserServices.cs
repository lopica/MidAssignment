using Microsoft.EntityFrameworkCore;
using MidAssignment.Domain;
using MidAssignment.DTOs;
using MidAssignment.DTOs.Category;
using MidAssignment.DTOs.User;
using MidAssignment.Repositories;
using MidAssignment.Services.Interfaces;
using System.Linq.Expressions;

namespace MidAssignment.Services
{
    public class ApplicationUserServices(IRepository<ApplicationUser> userRepo) : IApplicationUserServices
    {
        private readonly IRepository<ApplicationUser> _userRepo = userRepo;
        public Task<ApplicationResponse> DeleteAsync(string deleteId)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationResponse> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<ApplicationResponse> GetUsers(int currentPage, int limit, string? email = null)
        {
            if (currentPage <= 0) currentPage = 1;
            if (limit <= 0) limit = 5;

            // Build predicate
            Expression<Func<ApplicationUser, bool>> predicate = u => string.IsNullOrEmpty(email) || u.Email!.Equals(email);

            // Fetch filtered categories
            IQueryable<ApplicationUser> query = _userRepo.GetQueryable(predicate);

            int totalRecords = await query.AsNoTracking().CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalRecords / limit);

            if (currentPage > totalPages && totalPages > 0)
            {
                return new ErrorApplicationResponse(StatusCodes.Status400BadRequest, ["This page is out of scope"]);
            }

            var paginatedCategories = await query
                .Skip((currentPage - 1) * limit)
                .Take(limit)
                .Select(u => new UserDto(u.Id, u.Email!, u.AvatarUrl ?? string.Empty)) 
                .AsNoTracking()
                .ToListAsync();

            var result = new PaginateDto<UserDto>(
                paginatedCategories,
                totalPages,
                currentPage
               );

            return new SuccessApplicationResponse<PaginateDto<UserDto>>(StatusCodes.Status200OK, result);
        }

        public Task<ApplicationResponse> UpdateAsync(string updateId, UpdateApplicationUserDto updateUserDto)
        {
            throw new NotImplementedException();
        }
    }
}
