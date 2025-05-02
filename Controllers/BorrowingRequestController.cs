using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MidAssignment.DTOs;
using MidAssignment.DTOs.BorrowingRequest;
using MidAssignment.Services.Interfaces;

namespace MidAssignment.Controllers
{
    [ApiController]
    [Route("api/borrowing-request")]
    public class BorrowingRequestController(IBorrowingRequestServices requestServices) : Controller
    {
        private readonly IBorrowingRequestServices _requestServices = requestServices;
        [HttpPost]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.CreatedApplicationResponse<CreateRequestDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.BadErrorApplicationResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.NotFoundApplicationResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.ViolateBusinessApplicationResponse), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.InternalErrorApplicationResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateNewBorrowingRequest([FromBody] CreateRequestDto newRequest)
        {
            // check if list book is empty or above 5
            if (newRequest.BookIds.Count > 5)
            {
                return UnprocessableEntity(new ErrorApplicationResponse(StatusCodes.Status422UnprocessableEntity, ["Maximum 5 books per request"]));
            }
            if (newRequest.BookIds.Count == 0)
            {
                return UnprocessableEntity(new ErrorApplicationResponse(StatusCodes.Status422UnprocessableEntity, ["At least 1 book per request"]));
            }
            if (newRequest.BookIds.Count != newRequest.BookIds.Distinct().Count())
            {
                return UnprocessableEntity(new ErrorApplicationResponse(StatusCodes.Status422UnprocessableEntity, ["Should not borrow the same book per request"]));
            }
            if (newRequest.DueDate < DateTime.UtcNow)
            {
                return UnprocessableEntity(new ErrorApplicationResponse(StatusCodes.Status422UnprocessableEntity, ["DueDate must be in the future"]));
            }
            var result = await _requestServices.CreateNewRequest(newRequest);
            if (result.Success)
            {
                return StatusCode(StatusCodes.Status201Created, result);
            }
            if (result.StatusCode == StatusCodes.Status422UnprocessableEntity)
            {
                return UnprocessableEntity(result);
            }
            if (result.StatusCode == StatusCodes.Status404NotFound)
            {
                return NotFound(result);
            }
            if (result.StatusCode == StatusCodes.Status400BadRequest)
            {
                return BadRequest(result);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.SuccessApplicationResponse<PaginateDto<RequestDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.BadErrorApplicationResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.NotFoundApplicationResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.InternalErrorApplicationResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBorrowingRequests([FromQuery] int currentPage = 1, [FromQuery] int limit = 5, string? userId = null)
        {
            var result = await _requestServices.GetRequests(currentPage, limit, userId);
            if (result.Success)
            {
                return Ok(result);
            }
            if (result.StatusCode == StatusCodes.Status404NotFound)
            {
                return NotFound(result);
            }
            if (result.StatusCode == StatusCodes.Status400BadRequest)
            {
                return BadRequest(result);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.SuccessApplicationResponse<RequestDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.NotFoundApplicationResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.InternalErrorApplicationResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBorrowingRequestById(Guid id)
        {
            ApplicationResponse result = await _requestServices.GetRequestById(id);
            if (result.Success)
            {
                return Ok(result);
            }
            if (result.StatusCode == StatusCodes.Status404NotFound)
            {
                return NotFound(result);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }

        [HttpPut("{updateId}")]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.SuccessApplicationResponse<RequestDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.BadErrorApplicationResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.NotFoundApplicationResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.ViolateBusinessApplicationResponse), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.InternalErrorApplicationResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateBorrowingRequestById(string updateId, [FromBody] UpdateRequestDto updateRequest)
        {
            if (updateRequest.BookIds.Count > 5)
            {
                return UnprocessableEntity(new ErrorApplicationResponse(StatusCodes.Status422UnprocessableEntity, ["Maximum 5 books per request"]));
            }
            if (updateRequest.BookIds.Count == 0)
            {
                return UnprocessableEntity(new ErrorApplicationResponse(StatusCodes.Status422UnprocessableEntity, ["At least 1 book per request"]));
            }
            if (updateRequest.BookIds.Count != updateRequest.BookIds.Distinct().Count())
            {
                return UnprocessableEntity(new ErrorApplicationResponse(StatusCodes.Status422UnprocessableEntity, ["Should not borrow the same book per request"]));
            }
            if (updateRequest.DueDate < DateTime.UtcNow)
            {
                return UnprocessableEntity(new ErrorApplicationResponse(StatusCodes.Status422UnprocessableEntity, ["DueDate must be in the future"]));
            }
            ApplicationResponse result = await _requestServices.UpdateRequestById(Guid.Parse(updateId), updateRequest);
            if (result.Success)
            {
                return Ok(result);
            }
            if (result.StatusCode == StatusCodes.Status404NotFound)
            {
                return NotFound(result);
            }
            if (result.StatusCode == StatusCodes.Status422UnprocessableEntity)
            {
                return UnprocessableEntity(result);
            }
            if (result.StatusCode == StatusCodes.Status400BadRequest)
            {
                return BadRequest(result);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }

        [HttpDelete("{deleteId}")]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.SuccessApplicationResponse<RequestDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.NotFoundApplicationResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.ViolateBusinessApplicationResponse), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.InternalErrorApplicationResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteBorrowingRequestById(Guid deleteId)
        {
            ApplicationResponse result = await _requestServices.DeleteRequestById(deleteId);
            if (result.Success)
            {
                return Ok(result);
            }
            if (result.StatusCode == StatusCodes.Status404NotFound)
            {
                return NotFound(result);
            }
            if (result.StatusCode == StatusCodes.Status422UnprocessableEntity)
            {
                return UnprocessableEntity(result);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }
    }
}
