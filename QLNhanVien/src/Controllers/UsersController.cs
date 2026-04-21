using Microsoft.AspNetCore.Mvc;
using QLNhanVien.src.Models.DTOs;
using QLNhanVien.src.Services;

namespace QLNhanVien.src.Controllers;

/// <summary>
/// UsersController - API endpoints cho User management
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Lấy danh sách nhân viên với pagination
    /// </summary>
    /// <param name="page">Trang (từ 1)</param>
    /// <param name="pageSize">Số item mỗi trang (max 100)</param>
    /// <param name="search">Từ khóa tìm kiếm (tên, email, mã nhân viên)</param>
    /// <returns>Danh sách nhân viên phân trang</returns>
    /// <response code="200">Success</response>
    /// <response code="400">Invalid parameters</response>
    /// <response code="500">Server error</response>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaginatedResponse<UserDto>>> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        try
        {
            _logger.LogInformation("GET /api/users - page={Page}, pageSize={PageSize}, search={Search}", page, pageSize, search);

            if (page < 1)
                return BadRequest(new ApiErrorResponse
                {
                    Message = "Page must be greater than 0",
                    Errors = new() { { "page", new List<string> { "Page must be >= 1" } } }
                });

            if (pageSize < 1 || pageSize > 100)
                return BadRequest(new ApiErrorResponse
                {
                    Message = "Page size must be between 1 and 100",
                    Errors = new() { { "pageSize", new List<string> { "Page size must be between 1 and 100" } } }
                });

            var result = await _userService.GetUsersPagedAsync(page, pageSize, search);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
            {
                Message = "Error retrieving users",
                Errors = new() { { "internal", new List<string> { ex.Message } } }
            });
        }
    }

    /// <summary>
    /// Lấy thông tin chi tiết nhân viên
    /// </summary>
    /// <param name="id">ID nhân viên</param>
    /// <returns>Thông tin nhân viên</returns>
    /// <response code="200">Success</response>
    /// <response code="404">User not found</response>
    /// <response code="500">Server error</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetUserById(int id)
    {
        try
        {
            _logger.LogInformation("GET /api/users/{Id}", id);

            if (id <= 0)
                return BadRequest(new ApiErrorResponse { Message = "Invalid user id" });

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new ApiErrorResponse { Message = $"User with id {id} not found" });

            return Ok(new ApiResponse<UserDto>
            {
                Success = true,
                Data = user,
                Message = "Success"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
            {
                Message = "Error retrieving user",
                Errors = new() { { "internal", new List<string> { ex.Message } } }
            });
        }
    }

    /// <summary>
    /// Tạo nhân viên mới
    /// </summary>
    /// <param name="request">Thông tin nhân viên mới</param>
    /// <returns>Nhân viên vừa tạo</returns>
    /// <response code="201">Created</response>
    /// <response code="400">Invalid input</response>
    /// <response code="409">Email or employee code already exists</response>
    /// <response code="500">Server error</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<UserDto>>> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            _logger.LogInformation("POST /api/users - Creating user: {Email}", request.Email);

            if (request == null)
                return BadRequest(new ApiErrorResponse { Message = "Request body is null" });

            if (!ModelState.IsValid)
                return BadRequest(new ApiErrorResponse
                {
                    Message = "Invalid request",
                    Errors = ModelState
                        .Where(m => m.Value != null && m.Value.Errors.Count > 0)
                        .ToDictionary(
                            m => m.Key,
                            m => m.Value!.Errors.Select(e => e.ErrorMessage).ToList())
                });

            var user = await _userService.CreateUserAsync(request);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, new ApiResponse<UserDto>
            {
                Success = true,
                Data = user,
                Message = "User created successfully"
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Validation error creating user");
            var statusCode = ex.Message.Contains("already exists")
                ? StatusCodes.Status409Conflict
                : StatusCodes.Status400BadRequest;

            return StatusCode(statusCode, new ApiErrorResponse
            {
                Message = ex.Message,
                Errors = new() { { "validation", new List<string> { ex.Message } } }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
            {
                Message = "Error creating user",
                Errors = new() { { "internal", new List<string> { ex.Message } } }
            });
        }
    }

    /// <summary>
    /// Cập nhật thông tin nhân viên
    /// </summary>
    /// <param name="id">ID nhân viên</param>
    /// <param name="request">Thông tin cập nhật</param>
    /// <returns>Nhân viên sau khi cập nhật</returns>
    /// <response code="200">Success</response>
    /// <response code="404">User not found</response>
    /// <response code="409">Email already exists</response>
    /// <response code="500">Server error</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<UserDto>>> UpdateUser(int id, [FromBody] UpdateUserRequest request)
    {
        try
        {
            _logger.LogInformation("PUT /api/users/{Id}", id);

            if (id <= 0)
                return BadRequest(new ApiErrorResponse { Message = "Invalid user id" });

            var user = await _userService.UpdateUserAsync(id, request);
            return Ok(new ApiResponse<UserDto>
            {
                Success = true,
                Data = user,
                Message = "User updated successfully"
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error updating user {Id}", id);
            var statusCode = ex.Message.Contains("not found")
                ? StatusCodes.Status404NotFound
                : (ex.Message.Contains("already exists") ? StatusCodes.Status409Conflict : StatusCodes.Status400BadRequest);

            return StatusCode(statusCode, new ApiErrorResponse
            {
                Message = ex.Message,
                Errors = new() { { "validation", new List<string> { ex.Message } } }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
            {
                Message = "Error updating user",
                Errors = new() { { "internal", new List<string> { ex.Message } } }
            });
        }
    }

    /// <summary>
    /// Xóa nhân viên (soft delete)
    /// </summary>
    /// <param name="id">ID nhân viên</param>
    /// <returns>No content</returns>
    /// <response code="204">No content</response>
    /// <response code="404">User not found</response>
    /// <response code="500">Server error</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            _logger.LogInformation("DELETE /api/users/{Id}", id);

            if (id <= 0)
                return BadRequest(new ApiErrorResponse { Message = "Invalid user id" });

            var result = await _userService.DeleteUserAsync(id);
            if (!result)
                return NotFound(new ApiErrorResponse { Message = $"User with id {id} not found" });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
            {
                Message = "Error deleting user",
                Errors = new() { { "internal", new List<string> { ex.Message } } }
            });
        }
    }

    /// <summary>
    /// Lấy danh sách nhân viên đang hoạt động
    /// </summary>
    /// <returns>Danh sách nhân viên đang hoạt động</returns>
    /// <response code="200">Success</response>
    /// <response code="500">Server error</response>
    [HttpGet("active/list")]
    [ProducesResponseType(typeof(ApiResponse<List<UserDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<List<UserDto>>>> GetActiveUsers()
    {
        try
        {
            _logger.LogInformation("GET /api/users/active/list");
            var users = await _userService.GetAllActiveUsersAsync();
            return Ok(new ApiResponse<List<UserDto>>
            {
                Success = true,
                Data = users,
                Message = "Success"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active users");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
            {
                Message = "Error retrieving active users",
                Errors = new() { { "internal", new List<string> { ex.Message } } }
            });
        }
    }
}
