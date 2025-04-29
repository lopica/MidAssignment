using MidAssignment.DTOs;
using MidAssignment.DTOs.Book;

namespace MidAssignment.Services.Interfaces
{
    public interface IBookServices
    {
        Task<ApplicationResponse> CreateNewBook(CreateBookDto newBook);
        Task<ApplicationResponse> GetBooks(int currentPage, int limit, string? title = null);
        Task<ApplicationResponse> GetBookById(Guid id);
        Task<ApplicationResponse> UpdateBookById(Guid updateId, UpdateBookDto updateBook);
        Task<ApplicationResponse> DeleteBookById(Guid deleteId);
    }
}
