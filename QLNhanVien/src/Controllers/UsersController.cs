using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLNhanVien.src.Models.DTOs;
using QLNhanVien.src.Services;

namespace QLNhanVien.src.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<UserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResponse<UserDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? keyword = null,
        [FromQuery] string? status = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetAllAsync(new UserQueryRequest
        {
            Page = page,
            PageSize = pageSize,
            Keyword = keyword,
            Status = status
        }, cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetDetail(int id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetDetailAsync(id, cancellationToken);
        if (user == null)
        {
            return NotFound(new ApiErrorResponse { Message = "User not found" });
        }

        return Ok(new ApiResponse<UserDto>
        {
            Success = true,
            Data = user,
            Message = "Success"
        });
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<UserDto>>> Create([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var created = await _userService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetDetail), new { id = created.Id }, new ApiResponse<UserDto>
            {
                Success = true,
                Data = created,
                Message = "Created"
            });
        }
        catch (InvalidOperationException ex)
        {
            var statusCode = ex.Message.Contains("exists", StringComparison.OrdinalIgnoreCase)
                ? StatusCodes.Status409Conflict
                : StatusCodes.Status400BadRequest;

            return StatusCode(statusCode, new ApiErrorResponse
            {
                Message = ex.Message,
                Errors = new Dictionary<string, List<string>>
                {
                    ["validation"] = new List<string> { ex.Message }
                }
            });
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _userService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound(new ApiErrorResponse { Message = "User not found" });
        }

        return NoContent();
    }

    [HttpDelete]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteMany([FromBody] DeleteManyUsersRequest request, CancellationToken cancellationToken)
    {
        if (request.UserIds.Count == 0)
        {
            return BadRequest(new ApiErrorResponse { Message = "UserIds is required" });
        }

        var affected = await _userService.DeleteManyAsync(request.UserIds, cancellationToken);
        return Ok(new ApiResponse<object>
        {
            Success = true,
            Data = new { deletedCount = affected },
            Message = "Success"
        });
    }
}
