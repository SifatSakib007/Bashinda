using BashindaAPI.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BashindaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        // Common functionality for all API controllers

        protected ActionResult<ApiResponse<T>> OkWithResponse<T>(T data)
        {
            return Ok(ApiResponse<T>.SuccessResponse(data));
        }

        protected ActionResult<ApiResponse<T>> BadRequestWithResponse<T>(params string[] errors)
        {
            return BadRequest(ApiResponse<T>.ErrorResponse(errors));
        }

        protected ActionResult<ApiResponse<T>> NotFoundWithResponse<T>(params string[] errors)
        {
            return NotFound(ApiResponse<T>.ErrorResponse(errors));
        }

        protected ActionResult<ApiResponse<T>> ServerErrorWithResponse<T>(params string[] errors)
        {
            return StatusCode(500, ApiResponse<T>.ErrorResponse(errors));
        }
    }
}
