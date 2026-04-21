using Microsoft.AspNetCore.Mvc;
using QLNhanVien.src.Auth;
using QLNhanVien.src.Models.DTOs;

namespace QLNhanVien.src.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<UserDto>>> Register([FromBody] AuthRegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _authService.RegisterAsync(request, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, new ApiResponse<UserDto>
            {
                Success = true,
                Data = user,
                Message = "Register success"
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiErrorResponse
            {
                Message = ex.Message,
                Errors = new Dictionary<string, List<string>>
                {
                    ["validation"] = new List<string> { ex.Message }
                }
            });
        }
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthLoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<AuthLoginResponse>>> Login([FromBody] AuthLoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authService.LoginAsync(request, cancellationToken);
            return Ok(new ApiResponse<AuthLoginResponse>
            {
                Success = true,
                Data = result,
                Message = "Login success"
            });
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(new ApiErrorResponse
            {
                Message = ex.Message,
                Errors = new Dictionary<string, List<string>>
                {
                    ["auth"] = new List<string> { ex.Message }
                }
            });
        }
    }

    [HttpPost("logout")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> Logout()
    {
        await _authService.LogoutAsync();
        return Ok(new ApiResponse<object>
        {
            Success = true,
            Data = new { },
            Message = "Logout success"
        });
    }
}
