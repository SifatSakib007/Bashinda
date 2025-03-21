using BashindaAPI.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BashindaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        // Common functionality for all API controllers

        protected ActionResult<ApiResponse<T>> OkWithResponse<T>(T data, string? message = null)
        {
            return Ok(ApiResponse<T>.SuccessResponse(data, message));
        }

        protected ActionResult<ApiResponse<T>> BadRequestWithResponse<T>(string? message = null, params string[] errors)
        {
            return BadRequest(ApiResponse<T>.ErrorResponse(message, errors));
        }

        protected ActionResult<ApiResponse<T>> NotFoundWithResponse<T>(string? message = null, params string[] errors)
        {
            return NotFound(ApiResponse<T>.ErrorResponse(message, errors));
        }

        protected ActionResult<ApiResponse<T>> ServerErrorWithResponse<T>(string? message = null, params string[] errors)
        {
            return StatusCode(500, ApiResponse<T>.ErrorResponse(message, errors));
        }

        protected ActionResult<ApiResponse<T>> ForbidWithResponse<T>(string message)
        {
            return new ObjectResult(new ApiResponse<T>
            {
                Success = false,
                Errors = new[] { message }
            })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }
    }
}
