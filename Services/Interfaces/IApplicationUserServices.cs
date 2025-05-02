using MidAssignment.DTOs;
using MidAssignment.DTOs.User;

namespace MidAssignment.Services.Interfaces
{
    public interface IApplicationUserServices
    {
        Task<ApplicationResponse> GetUsers(int currentPage, int limit, string? email = null);
        Task<ApplicationResponse> GetByIdAsync(string id);
        Task<ApplicationResponse> UpdateAsync(string updateId, UpdateApplicationUserDto updateUserDto);
        Task<ApplicationResponse> DeleteAsync(string deleteId);
    }
}
