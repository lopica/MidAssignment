using Microsoft.AspNetCore.Mvc;
using MidAssignment.DTOs;

namespace MidAssignment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BorrowingRequestController : Controller
    {
        [HttpPost]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.CreatedApplicationResponse<DTOs.SwaggerDTOs.CreateBorrowingRequestDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.BadErrorApplicationResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.NotFoundApplicationResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.ViolateBusinessApplicationResponse), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.InternalErrorApplicationResponse), StatusCodes.Status500InternalServerError)]
        public IActionResult CreateNewBorrowingRequest([FromBody] CreateBorrowingRequest newRequest)
        {
            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.SuccessApplicationResponse<PaginateDto<BorrowingRequestDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.BadErrorApplicationResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.NotFoundApplicationResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.InternalErrorApplicationResponse), StatusCodes.Status500InternalServerError)]
        public IActionResult GetBorrowingRequests([FromQuery] int currentPage = 1, [FromQuery] int limit = 10)
        {
            return Ok();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.SuccessApplicationResponse<BorrowingRequestDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.NotFoundApplicationResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.InternalErrorApplicationResponse), StatusCodes.Status500InternalServerError)]
        public IActionResult GetBorrowingRequestById(Guid id)
        {
            return Ok();
        }

        [HttpPut("{updateId}")]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.SuccessApplicationResponse<BorrowingRequestDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.BadErrorApplicationResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.NotFoundApplicationResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.ViolateBusinessApplicationResponse), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.InternalErrorApplicationResponse), StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateBorrowingRequestById(string updateId, [FromBody] CreateBorrowingRequest updateRequest)
        {
            return Ok();
        }

        [HttpDelete("{deleteId}")]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.SuccessApplicationResponse<BorrowingRequestDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.NotFoundApplicationResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.ViolateBusinessApplicationResponse), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.InternalErrorApplicationResponse), StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteBorrowingRequestById(string deleteId)
        {
            return Ok();
        }
    }
}
