using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLNhanVien.src.Models.DTOs;
using QLNhanVien.src.Services;
using System.Security.Claims;

namespace QLNhanVien.src.Controllers;

[ApiController]
[Authorize]
[Route("api/employees")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<EmployeeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResponse<EmployeeDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? keyword = null,
        [FromQuery] string? gioiTinh = null,
        [FromQuery] string? boPhan = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool descending = true,
        CancellationToken cancellationToken = default)
    {
        var query = new EmployeeQueryRequest
        {
            Page = page,
            PageSize = pageSize,
            Keyword = keyword,
            GioiTinh = gioiTinh,
            BoPhan = boPhan,
            SortBy = sortBy,
            Descending = descending
        };

        var result = await _employeeService.GetAllAsync(query, GetCurrentUserId(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> GetDetail(int id, CancellationToken cancellationToken)
    {
        var employee = await _employeeService.GetDetailAsync(id, cancellationToken);
        if (employee == null)
        {
            return NotFound(new ApiErrorResponse { Message = "Nhân viên không tồn tại" });
        }

        return Ok(new ApiResponse<EmployeeDto>
        {
            Success = true,
            Data = employee,
            Message = "Success"
        });
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<EmployeeDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> Create(
        [FromBody] CreateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var created = await _employeeService.CreateAsync(request, GetCurrentUserId(), cancellationToken);
            return CreatedAtAction(nameof(GetDetail), new { id = created.Id }, new ApiResponse<EmployeeDto>
            {
                Success = true,
                Data = created,
                Message = "Tạo nhân viên thành công"
            });
        }
        catch (InvalidOperationException ex)
        {
            var statusCode = ex.Message.Contains("tồn tại", StringComparison.OrdinalIgnoreCase)
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

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> Update(
        int id,
        [FromBody] UpdateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _employeeService.UpdateAsync(id, request, GetCurrentUserId(), cancellationToken);
            return Ok(new ApiResponse<EmployeeDto>
            {
                Success = true,
                Data = updated,
                Message = "Cập nhật nhân viên thành công"
            });
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(StatusCodes.Status404NotFound, new ApiErrorResponse
            {
                Message = ex.Message,
                Errors = new Dictionary<string, List<string>>
                {
                    ["error"] = new List<string> { ex.Message }
                }
            });
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _employeeService.DeleteAsync(id, GetCurrentUserId(), cancellationToken);
        if (!deleted)
        {
            return NotFound(new ApiErrorResponse { Message = "Nhân viên không tồn tại" });
        }

        return NoContent();
    }

    [HttpDelete]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteMany(
        [FromBody] DeleteManyEmployeesRequest request,
        CancellationToken cancellationToken)
    {
        if (request.EmployeeIds.Count == 0)
        {
            return BadRequest(new ApiErrorResponse { Message = "EmployeeIds không được để trống" });
        }

        var affected = await _employeeService.DeleteManyAsync(request.EmployeeIds, GetCurrentUserId(), cancellationToken);
        return Ok(new ApiResponse<object>
        {
            Success = true,
            Data = new { deletedCount = affected },
            Message = "Success"
        });
    }
}
