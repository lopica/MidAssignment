using Microsoft.EntityFrameworkCore;
using MidAssignment.Domain;
using MidAssignment.DTOs;
using MidAssignment.DTOs.Book;
using MidAssignment.DTOs.Category;
using MidAssignment.Repositories;
using MidAssignment.Services.Interfaces;
using System.Linq.Expressions;

namespace MidAssignment.Services
{
    public class BookServices(IRepository<Book> bookRepo, IRepository<Category> cateRepo) : IBookServices
    {
        private readonly IRepository<Book> _bookRepo = bookRepo;
        private readonly IRepository<Category> _cateRepo = cateRepo;

        public async Task<ApplicationResponse> CreateNewBook(CreateBookDto newBookDto)
        {
            var bookExists = _bookRepo.GetQueryable(b => b.Title == newBookDto.Title);
            if (await bookExists.AnyAsync())
            {
                return new ErrorApplicationResponse(StatusCodes.Status400BadRequest, [$"Book title {newBookDto.Title} is already exist"]);
            }

            Book newBook = new()
            {
                Title = newBookDto.Title,
                Author = newBookDto.Author,
                EditionNumber = newBookDto.EditionNumber,
                Categories = []
            };

            // Check if categories are provided and map them to the book
            if (newBookDto.CategoryIds != null && newBookDto.CategoryIds.Count != 0)
            {
                foreach (var categoryId in newBookDto.CategoryIds)
                {
                    Category? existCategory = await _cateRepo.GetByIdAsync(categoryId);
                    if (existCategory == null)
                    {
                        return new ErrorApplicationResponse(StatusCodes.Status404NotFound, [$"Category with ID {categoryId} does not exist."]);
                    }
                    newBook.Categories.Add(existCategory);
                }
            }

            // Add the book to the repository
            await _bookRepo.AddAsync(newBook);

            // Save changes to the database
            var success = await _bookRepo.SaveChangesAsync();

            var bookResponse = new BookDto(newBook.Id, newBook.Title, newBook.Author, newBook.EditionNumber, newBook.Quantity, newBook.IsAvailable,
                newBook.Categories.Select(b => b.Id).ToList());

            return new SuccessApplicationResponse<BookDto>(StatusCodes.Status201Created, bookResponse);
        }


        public async Task<ApplicationResponse> DeleteBookById(Guid deleteId)
        {
            // Find the category by ID
            Book? existBook = await _bookRepo.GetByIdWithIncludesAsync(deleteId, b => b.Categories!);
            if (existBook == null)
            {
                return new ErrorApplicationResponse(StatusCodes.Status404NotFound, [$"Book id {deleteId} is not found"]);
            }

            // Delete the category
            await _bookRepo.Delete(deleteId);
            var result = await _bookRepo.SaveChangesAsync();

            if (result)
            {
                BookDto bookResponse = new(existBook.Id, existBook.Title, existBook.Author, existBook.EditionNumber, existBook.Quantity, existBook.IsAvailable, existBook.Categories!.Select(c => c.Id).ToList());
                return new SuccessApplicationResponse<BookDto>(StatusCodes.Status200OK, bookResponse);
            }
            else
            {
                return new ErrorApplicationResponse(StatusCodes.Status500InternalServerError, ["Please try again later"]);
            }
        }

        public async Task<ApplicationResponse> GetBookById(Guid id)
        {
            Book? matchBook = await _bookRepo.GetByIdWithIncludesAsync(id, b => b.Categories!);
            if (matchBook == null)
            {
                return new ErrorApplicationResponse(StatusCodes.Status404NotFound, ["Category not found"]);
            }
            var bookResponse = new BookDto(matchBook.Id, matchBook.Title, matchBook.Author, matchBook.EditionNumber, matchBook.Quantity,matchBook.IsAvailable, matchBook.Categories != null ? matchBook.Categories.Select(c => c.Id).ToList() : []);
            return new SuccessApplicationResponse<BookDto>(StatusCodes.Status200OK, bookResponse);
        }

        public async Task<ApplicationResponse> GetBooks(int currentPage, int limit, string? title = null)
        {
            if (currentPage <= 0) currentPage = 1;
            if (limit <= 0) limit = 5;

            // Build predicate for filtering by title
            Expression<Func<Book, bool>> predicate = c => string.IsNullOrEmpty(title) || c.Title.Contains(title);

            // Fetch books with Category IDs eagerly loaded
            var allBooks = _bookRepo.GetQueryable(predicate)
                .Include(b => b.Categories) 
                .AsNoTracking();

            int totalRecords = await allBooks.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalRecords / limit);

            if (currentPage > totalPages && totalPages > 0)
            {
                return new ErrorApplicationResponse(StatusCodes.Status400BadRequest, ["This page is out of scope"]);
            }

            var paginatedBooks = await allBooks
               .Select(b => new BookDto(
                   b.Id,
                   b.Title,
                   b.Author,
                   b.EditionNumber,
                   b.Quantity,
                   b.IsAvailable,
                   b.Categories != null ? b.Categories.Select(c => c.Id).ToList() : new List<Guid>()
               ))
               .Skip((currentPage - 1) * limit)
               .Take(limit)
               .ToListAsync();

            var result = new PaginateDto<BookDto>(
                paginatedBooks,
                totalPages,
                currentPage
            );

            return new SuccessApplicationResponse<PaginateDto<BookDto>>(StatusCodes.Status200OK, result);
        }

        //working with this method
        public async Task<ApplicationResponse> UpdateBookById(Guid updateId, UpdateBookDto updateBook)
        {
            // Find the book by id
            var existBook = await _bookRepo.GetByIdWithIncludesAsync(updateId, b => b.Categories!);
            if (existBook == null)
            {
                return new ErrorApplicationResponse(StatusCodes.Status404NotFound, [$"Book with ID {updateId} not found"]);
            }
            existBook.Categories?.Clear();
            if (updateBook.CategoryIds != null && updateBook.CategoryIds.Count != 0)
            {
                foreach (var categoryId in updateBook.CategoryIds)
                {
                    Category? category = await _cateRepo.GetByIdAsync(categoryId);
                    if (category == null)
                    {
                        return new ErrorApplicationResponse(StatusCodes.Status404NotFound, [$"Category with ID {categoryId} does not exist."]);
                    }
                    existBook.Categories?.Add(category);
                }
            }


            // Update the properties
            existBook.Title = updateBook.Title;
            existBook.Author = updateBook.Author;
            existBook.EditionNumber = updateBook.EditionNumber;
            existBook.Quantity = updateBook.Quantity;
            existBook.IsAvailable = updateBook.IsAvailable;

            _bookRepo.Update(existBook);
            var result = await _bookRepo.SaveChangesAsync();

            if (result)
            {
                BookDto updatedBook = new(existBook.Id, existBook.Title, existBook.Author, existBook.EditionNumber, existBook.Quantity, existBook.IsAvailable, existBook.Categories!.Select(c => c.Id).ToList());
                return new SuccessApplicationResponse<BookDto>(StatusCodes.Status200OK, updatedBook);
            }
            else
            {
                return new ErrorApplicationResponse(StatusCodes.Status500InternalServerError, ["Please try again later"]);
            }
        }

    }
}
