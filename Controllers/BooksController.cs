using Microsoft.AspNetCore.Mvc;
using MidAssignment.DTOs;
using MidAssignment.DTOs.Book;
using MidAssignment.Services;
using MidAssignment.Services.Interfaces;

namespace MidAssignment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController(IBookServices bookServices) : Controller
    {
        private readonly IBookServices _bookServices = bookServices;
        [HttpPost]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.CreatedApplicationResponse<BookDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.BadErrorApplicationResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.NotFoundApplicationResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.InternalErrorApplicationResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateNewBook([FromBody] CreateBookDto newBook)
        {
            ApplicationResponse result = await _bookServices.CreateNewBook(newBook);
            if (result.Success)
            {
                return StatusCode(StatusCodes.Status201Created, result);
            }
            if (result.StatusCode == StatusCodes.Status400BadRequest)
            {
                return BadRequest(result);
            }
            return NotFound(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.SuccessApplicationResponse<PaginateDto<BookDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.BadErrorApplicationResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.InternalErrorApplicationResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBooks([FromQuery] int currentPage = 1, [FromQuery] int limit = 5)
        {
            ApplicationResponse result = await _bookServices.GetBooks(currentPage, limit);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.SuccessApplicationResponse<BookDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.NotFoundApplicationResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.InternalErrorApplicationResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBookById(Guid id)
        {
            ApplicationResponse result = await _bookServices.GetBookById(id);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPut("{updateId}")]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.SuccessApplicationResponse<BookDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.BadErrorApplicationResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.NotFoundApplicationResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.InternalErrorApplicationResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateBookById(Guid updateId, [FromBody] UpdateBookDto updateBook)
        {
            ApplicationResponse result = await _bookServices.UpdateBookById(updateId, updateBook);
            if (result.Success)
            {
                return Ok(result);
            }
            if (result.StatusCode == StatusCodes.Status400BadRequest)
            {
                return BadRequest(result);
            }
            return NotFound(result);
        }

        [HttpDelete("{deleteId}")]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.SuccessApplicationResponse<BookDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.NotFoundApplicationResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.ViolateBusinessApplicationResponse), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.InternalErrorApplicationResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteBookById(Guid deleteId)
        {
            ApplicationResponse result = await _bookServices.DeleteBookById(deleteId);
            if (result.Success)
            {
                return Ok(result);
            }
            if (result.StatusCode == StatusCodes.Status404NotFound)
            {
                return NotFound(result);
            }
            return UnprocessableEntity(result);
        }
    }
}
