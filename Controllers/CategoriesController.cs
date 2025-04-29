using Microsoft.AspNetCore.Mvc;
using MidAssignment.DTOs;
using MidAssignment.DTOs.Category;
using MidAssignment.Services.Interfaces;

namespace MidAssignment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController(ICategoryServices categoryServices) : Controller
    {
        private readonly ICategoryServices _categoryServices = categoryServices;
        [HttpPost]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.CreatedApplicationResponse<CategoryDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.BadErrorApplicationResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.InternalErrorApplicationResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateNewCategory([FromBody] CreateCategoryDto newCategory)
        {
            ApplicationResponse result = await _categoryServices.CreateNewCategory(newCategory);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.SuccessApplicationResponse<PaginateDto<CategoryDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.BadErrorApplicationResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.NotFoundApplicationResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.InternalErrorApplicationResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCategories([FromQuery] int currentPage = 1, [FromQuery] int limit = 5)
        {
            return Ok(await _categoryServices.GetCategories(currentPage, limit));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.SuccessApplicationResponse<CategoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.NotFoundApplicationResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.InternalErrorApplicationResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            ApplicationResponse result = await _categoryServices.GetCategoryById(id);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPut("{updateId}")]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.SuccessApplicationResponse<CategoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.BadErrorApplicationResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.NotFoundApplicationResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.InternalErrorApplicationResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCategoryById(Guid updateId, [FromBody] CreateCategoryDto updateCategory)
        {
            ApplicationResponse result = await _categoryServices.UpdateCategoryById(updateId, updateCategory);
            if (result.Success)
            {
                return Ok(result);
            }
            if (result.StatusCode == StatusCodes.Status404NotFound)
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }

        [HttpDelete("{deleteId}")]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.SuccessApplicationResponse<CategoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.NotFoundApplicationResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DTOs.SwaggerDTOs.InternalErrorApplicationResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCategoryById(Guid deleteId)
        {
            ApplicationResponse result = await _categoryServices.DeleteCategoryById(deleteId);
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
