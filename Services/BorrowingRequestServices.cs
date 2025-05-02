using Microsoft.EntityFrameworkCore;
using MidAssignment.Domain;
using MidAssignment.DTOs;
using MidAssignment.DTOs.BorrowingRequest;
using MidAssignment.Repositories;
using MidAssignment.Services.Interfaces;
using System.Linq.Expressions;
using System.Net;

namespace MidAssignment.Services
{
    public class BorrowingRequestServices(
        IRepository<BookBorrowingRequest> requestRepo,
        IRepository<BookBorrowingRequestDetail> requestDetailRepo,
        IRepository<ApplicationUser> userRepo,
        IRepository<Book> bookRepo
        ) : IBorrowingRequestServices
    {
        private readonly IRepository<BookBorrowingRequest> _requestRepo = requestRepo;
        private readonly IRepository<BookBorrowingRequestDetail> _requestDetailRepo = requestDetailRepo;
        private readonly IRepository<ApplicationUser> _userRepo = userRepo;
        private readonly IRepository<Book> _bookRepo = bookRepo;

        public async Task<ApplicationResponse> CreateNewRequest(CreateRequestDto newRequest)
        {
            // Check if the user exists
            ApplicationUser? existUser = await _userRepo.GetByIdAsync(newRequest.RequestorId);
            if (existUser == null)
            {
                return new ErrorApplicationResponse(StatusCodes.Status404NotFound, [$"User with id {newRequest.RequestorId} not found"]);
            }
            // Check if the books exist
            List<Book> existBooks = await _bookRepo
                .GetQueryable(b => newRequest.BookIds.Contains(b.Id))
                .ToListAsync();
            List<Guid> existBookIds = existBooks.Select(b => b.Id).ToList();
            if (existBookIds.Count != newRequest.BookIds.Count)
            {
                List<Guid> missingBookIds = newRequest.BookIds.Where(id => !existBookIds.Contains(id)).ToList();
                return new ErrorApplicationResponse(StatusCodes.Status404NotFound,
                    [$"The following book ID{(missingBookIds.Count == 1 ? "" : "s")} do not exist: {string.Join(", ", missingBookIds)}"]);
            }
            // Check if the books are available
            List<Book> unavailableBooks = existBooks.Where(b => b.IsAvailable == false || b.Quantity == 0).ToList();
            if (unavailableBooks.Count > 0)
            {
                List<Guid> unavailableBookIds = unavailableBooks.Select(b => b.Id).ToList();
                return new ErrorApplicationResponse(StatusCodes.Status400BadRequest,
                    [$"The following book ID{(unavailableBookIds.Count == 1 ? "" : "s")} are not available: {string.Join(", ", unavailableBookIds)}"]);
            }

            // check if there are 3 request has createAt in 1 month 
            var requestThisMonth = await _requestRepo
                .GetQueryable(r => r.RequestorId == newRequest.RequestorId && r.RequestDate >= DateTime.Now.AddMonths(-1))
                .Include(r => r.BookBorrowingRequestDetail)
                    .ThenInclude(d => d.Books)
                .ToListAsync();

            if (requestThisMonth.Count > 3)
            {
                return new ErrorApplicationResponse(StatusCodes.Status422UnprocessableEntity, ["Cannot create more than 3 request per month. Please wait until next month."]);
            }
            List<Guid> borrowedBookIds = requestThisMonth
                .Where(r => r.BookBorrowingRequestDetail != null)
                .SelectMany(r => r.BookBorrowingRequestDetail!.Books)
                .Select(book => book.Id)
                .ToList();

            List<Guid> borrowedIds = borrowedBookIds.Intersect(existBookIds).ToList();

            //List<BookBorrowingRequest> matchRequestThisMonth = requestThisMonth.Where(r => existBookIds.Contains(r.BookBorrowingRequestDetail.Books)).ToList();
            // check if in those request contain book that have been request
            if (borrowedIds.Count > 0)
            {
                return new ErrorApplicationResponse(StatusCodes.Status422UnprocessableEntity, [$"Cannot borrow the same book in month: {string.Join(", ", borrowedIds)}"]);
            }

            // Create a new request

            BookBorrowingRequest newBookRequest = new()
            {
                Requestor = existUser,
                Status = Status.Waiting,
            };
            // Create request details
            BookBorrowingRequestDetail newRequestDetail = new()
            {
                BookBorrowingRequest = newBookRequest,
                DueDate = newRequest.DueDate,
                Books = existBooks,
            };
            // Save the request and details to the database
            await _requestRepo.AddAsync(newBookRequest);
            await _requestDetailRepo.AddAsync(newRequestDetail);
            var result = await _requestRepo.SaveChangesAsync();
            //var result2 = await _requestDetailRepo.SaveChangesAsync();
            // Return a success response
            if (result)
            {
                RequestDto response = new(
                newBookRequest.Id,
                newBookRequest.RequestorId!,
                existBooks.Select(b => b.Id).ToList(),
                newBookRequest.Status,
                newRequestDetail.DueDate,
                newRequestDetail.UpdateAt
                );
                return new SuccessApplicationResponse<RequestDto>(StatusCodes.Status201Created, response);
            }
            else
            {
                return new ErrorApplicationResponse(StatusCodes.Status500InternalServerError, ["Failed to create request"]);
            }
        }

        public async Task<ApplicationResponse> DeleteRequestById(Guid deleteId)
        {
            // Check if request exists
            var request = await _requestRepo.GetByIdWithIncludesAsync(
                deleteId,
                r => r.BookBorrowingRequestDetail!,
                r => r.Requestor,
                r => r.Approver,
                r => r.BookBorrowingRequestDetail!.Books
            );
            if (request == null)
            {
                return new ErrorApplicationResponse(StatusCodes.Status404NotFound, [$"Request with ID {deleteId} not found"]);
            }

            // Prevent deletion if already approved
            if (request.Status == Status.Approved || request.Status == Status.Rejected)
            {
                return new ErrorApplicationResponse(StatusCodes.Status400BadRequest, ["Cannot delete an approved or rejected request"]);
            }

            await _requestRepo.Delete(deleteId);
            var result = await _requestRepo.SaveChangesAsync();
            var requestDto = new RequestDto(
                request.Id,
                request.Requestor.Email!,
                request.BookBorrowingRequestDetail!.Books.Select(b => b.Id).ToList(),
                request.Status,
                request.BookBorrowingRequestDetail.DueDate,
                request.BookBorrowingRequestDetail.UpdateAt,
                request.BookBorrowingRequestDetail.ReturnDate,
                request.Approver != null ? request.Approver.Email : null
            );
            return result
                ? new SuccessApplicationResponse<RequestDto>(StatusCodes.Status200OK, requestDto)
                : new ErrorApplicationResponse(StatusCodes.Status500InternalServerError, ["Failed to delete request"]);
        }

        public async Task<ApplicationResponse> GetRequestById(Guid id)
        {
            var matchRequest = await _requestRepo.GetByIdWithIncludesAsync(
                id,
                r => r.BookBorrowingRequestDetail!,
                r => r.Requestor,
                r => r.Approver,
                r =>r.BookBorrowingRequestDetail!.Books
            );

            if (matchRequest == null)
            {
                return new ErrorApplicationResponse(StatusCodes.Status404NotFound, [$"Request with ID {id} not found"]);
            }

            var detail = matchRequest.BookBorrowingRequestDetail!;

            var dto = new RequestDto(
                matchRequest.Id,
                matchRequest.Requestor.Email!,
                detail.Books?.Select(b => b.Id).ToList() ?? [],
                matchRequest.Status,
                detail.DueDate,
                detail.UpdateAt,
                detail.ReturnDate,
                matchRequest.Approver?.Email
            );

            return new SuccessApplicationResponse<RequestDto>(StatusCodes.Status200OK, dto);
        }


        public async Task<ApplicationResponse> GetRequests(int currentPage, int limit, string? userId = null)
        {
            // Validate the request
            if (currentPage <= 0) currentPage = 1;
            if (limit <= 0) limit = 5;
            // Check if the user exists
            if (userId != null)
            {
                ApplicationUser? existUser = await _userRepo.GetByIdAsync(userId);
                if (existUser == null)
                {
                    return new ErrorApplicationResponse(StatusCodes.Status404NotFound, [$"User with id {userId} not found"]);
                }
            }
            // Get the requests
            Expression<Func<BookBorrowingRequest, bool>> predicate = r => string.IsNullOrEmpty(userId) || r.RequestorId!.Contains(userId);
            // Check if the requests are already approved
            var allRequests = _requestRepo.GetQueryable(predicate)
                .Include(r => r.BookBorrowingRequestDetail)
                .AsNoTracking();

            int totalRecords = await allRequests.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalRecords / limit);

            if (currentPage > totalPages && totalPages > 0)
            {
                return new ErrorApplicationResponse(StatusCodes.Status400BadRequest, ["This page is out of scope"]);
            }

            var paginatedBooks = await allRequests
               .Select(b => new RequestDto(
                     b.Id,
                   b.Requestor.Email!,
                   b.BookBorrowingRequestDetail!.Books.Select(b => b.Id).ToList(),
                   b.Status,
                   b.BookBorrowingRequestDetail.DueDate,
                   b.BookBorrowingRequestDetail.UpdateAt,
                   b.BookBorrowingRequestDetail.ReturnDate,
                   b.Approver != null ? b.Approver.Email : null
                   ))
               .Skip((currentPage - 1) * limit)
               .Take(limit)
               .ToListAsync();

            var result = new PaginateDto<RequestDto>(
                paginatedBooks,
                totalPages,
                currentPage
            );

            // Return a success response
            return new SuccessApplicationResponse<PaginateDto<RequestDto>>(StatusCodes.Status200OK, result);

        }

        public async Task<ApplicationResponse> UpdateRequestById(Guid updateId, UpdateRequestDto updateDto)
        {
            var existRequest = await _requestRepo.GetByIdWithIncludesAsync(
                updateId,
                r => r.BookBorrowingRequestDetail!,
                r => r.Requestor,
                r => r.Approver,
                r => r.BookBorrowingRequestDetail!.Books
            );
            if (existRequest == null)
            {
                return new ErrorApplicationResponse(StatusCodes.Status404NotFound, [$"Request with ID {updateId} not found"]);
            }
            if (existRequest.Status == Status.Approved || existRequest.Status == Status.Rejected)
            {
                return new ErrorApplicationResponse(StatusCodes.Status422UnprocessableEntity, ["Cannot update an approved or rejected request"]);
            }

            List<Book> existBooks = await _bookRepo
                .GetQueryable(b => updateDto.BookIds.Contains(b.Id))
                .ToListAsync();
            List<Guid> existBookIds = existBooks.Select(b => b.Id).ToList();
            if (existBookIds.Count != updateDto.BookIds.Count)
            {
                List<Guid> missingBookIds = updateDto.BookIds.Where(id => !existBookIds.Contains(id)).ToList();
                return new ErrorApplicationResponse(StatusCodes.Status404NotFound,
                    [$"The following book ID{(missingBookIds.Count == 1 ? "" : "s")} do not exist: {string.Join(", ", missingBookIds)}"]);
            }
            List<Book> unavailableBooks = existBooks.Where(b => b.IsAvailable == false || b.Quantity == 0).ToList();
            if (unavailableBooks.Count > 0)
            {
                List<Guid> unavailableBookIds = unavailableBooks.Select(b => b.Id).ToList();
                return new ErrorApplicationResponse(StatusCodes.Status400BadRequest,
                    [$"The following book ID{(unavailableBookIds.Count == 1 ? "" : "s")} are not available: {string.Join(", ", unavailableBookIds)}"]);
            }

            // check if there are 3 request has createAt in 1 month 
            var requestThisMonth = await _requestRepo
                .GetQueryable(r => r.RequestorId == existRequest.RequestorId && r.RequestDate >= DateTime.Now.AddMonths(-1))
                .Include(r => r.BookBorrowingRequestDetail)
                    .ThenInclude(d => d.Books)
                .ToListAsync();

            if (requestThisMonth.Count > 3)
            {
                return new ErrorApplicationResponse(StatusCodes.Status422UnprocessableEntity, ["Cannot create more than 3 request per month. Please wait until next month."]);
            }
            List<Guid> borrowedBookIds = requestThisMonth
                .Where(r => r.BookBorrowingRequestDetail != null)
                .SelectMany(r => r.BookBorrowingRequestDetail!.Books)
                .Select(book => book.Id)
                .ToList();

            List<Guid> borrowedIds = borrowedBookIds.Intersect(existBookIds).ToList();
            if (borrowedIds.Count > 0)
            {
                return new ErrorApplicationResponse(StatusCodes.Status422UnprocessableEntity, [$"Cannot borrow the same book in month: {string.Join(", ", borrowedIds)}"]);
            }
            ApplicationUser? approvedUser = null;

            if (updateDto.Status != Status.Waiting)
            {
                if (string.IsNullOrEmpty(updateDto.ApproverId))
                {
                    return new ErrorApplicationResponse(StatusCodes.Status422UnprocessableEntity, ["ApproverId is need when updating status"]);
                }

                approvedUser = await _userRepo.GetByIdAsync(updateDto.ApproverId);

                if (approvedUser == null)
                {
                    return new ErrorApplicationResponse(StatusCodes.Status404NotFound, [$"User with ID {updateDto.ApproverId} not found"]);
                }
            }



            existRequest.BookBorrowingRequestDetail!.Books = existBooks;
            existRequest.BookBorrowingRequestDetail.DueDate = updateDto.DueDate;
            existRequest.Status = updateDto.Status;
            existRequest.Approver = approvedUser;
            existRequest.BookBorrowingRequestDetail.DueDate = updateDto.DueDate;
            existRequest.BookBorrowingRequestDetail.ReturnDate = updateDto.ReturnDate;
            existRequest.BookBorrowingRequestDetail.UpdateAt = DateTime.Now;
            _requestRepo.Update(existRequest);
            var result = await _requestRepo.SaveChangesAsync();
            if (result)
            {
                var dto = new RequestDto(
                    existRequest.Id,
                    existRequest.RequestorId!,
                    existBooks.Select(b => b.Id).ToList(),
                    existRequest.Status,
                    existRequest.BookBorrowingRequestDetail.DueDate,
                    existRequest.BookBorrowingRequestDetail.UpdateAt,
                    existRequest.BookBorrowingRequestDetail.ReturnDate,
                    existRequest.Approver != null ? existRequest.Approver.Email : null
                );
                return new SuccessApplicationResponse<RequestDto>(StatusCodes.Status200OK, dto);
            }
            else
            {
                return new ErrorApplicationResponse(StatusCodes.Status500InternalServerError, ["Failed to update request"]);
            }
        }

        
    }
}
