using MidAssignment.DTOs;
using MidAssignment.DTOs.Book;
using MidAssignment.DTOs.BorrowingRequest;

namespace MidAssignment.Services.Interfaces
{
    public interface IBorrowingRequestServices
    {
        Task<ApplicationResponse> CreateNewRequest(CreateRequestDto newRequest);
        Task<ApplicationResponse> GetRequests(int currentPage, int limit, string? userId = null);
        Task<ApplicationResponse> GetRequestById(Guid id);
        Task<ApplicationResponse> UpdateRequestById(Guid updateId, UpdateRequestDto updateBook);
        Task<ApplicationResponse> DeleteRequestById(Guid deleteId);
    }
}
