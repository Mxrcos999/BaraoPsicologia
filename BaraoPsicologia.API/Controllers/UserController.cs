using BaraoPsicologia.Application.Dto.Shared;
using BaraoPsicologia.Application.Dto.User;
using BaraoPsicologia.Application.Interfaces.Services;
using BaraoPsicologia.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BaraoPsicologia.API.Controllers;

[ApiController]
[Route("user")]
public sealed class UserController : ControllerBase
{
    private static readonly HashSet<string> ValidProfiles = new(StringComparer.OrdinalIgnoreCase)
    {
        "admin", "atendente", "aluno", "supervisor"
    };

    private readonly IIdentityService _identity;
    private readonly UserManager<ApplicationUser> _users;
    private readonly IEmailService _email;

    public UserController(IIdentityService identity, UserManager<ApplicationUser> users, IEmailService email)
    {
        _identity = identity;
        _users = users;
        _email = email;
    }

    [HttpPost("user-login")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
    {
        var response = await _identity.LoginAsync(request);
        if (!response.Success)
            return BadRequest(response);
        return Ok(response);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword
        ([FromBody] ForgotPasswordRequest model)
    {
        var response = await _identity.ForgotPassword(model);
        if (!response.Success)
            return BadRequest(response);
        return Ok(new { });
    }

    [HttpPost("post-user")]
    public async Task<ActionResult<BaseResponse<string>>> PostUser
        ([FromBody] PostUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Profile) || !ValidProfiles.Contains(request.Profile))
        {
            return BadRequest(new { errors = new Dictionary<string, string[]> { ["profile"] = new[] { "Perfil inválido." } } });
        }

        var user = new ApplicationUser
        {
            Name = request.Name,
            Email = request.Email,
            UserName = request.Username,
            Profile = request.Profile,
            EmailConfirmed = true
        };

        var pwd = IdentityService.GeneratePassword();
        var result = await _users.CreateAsync(user, pwd);
        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                errors = new Dictionary<string, string[]>
                {
                    ["user"] = result.Errors.Select(e => e.Description).ToArray()
                }
            });
        }

        await _email.SendPassword(user.Email!, user.Name, pwd);
        return Ok(new BaseResponse<string> { Data = pwd, Page = 0, PageSize = 0, TotalRecords = 0 });
    }

    [HttpGet("get-user-list")]
    public async Task<ActionResult<PagedResult<UserListItemResponse>>> GetUserList
        ([FromQuery] BaseGetRequest request, CancellationToken ct)
    {
        var (page, pageSize) = ListQueryHelpers.ReadPaging(Request.Query);
        var q = FilterUsers(request.SearchInput).AsNoTracking();
        var total = await q.CountAsync(ct);
        var data = await q.OrderBy(u => u.Name)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(u => new UserListItemResponse
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email ?? "",
                Username = u.UserName ?? "",
                Profile = u.Profile,
                ReceiveEmails = u.ReceiveEmails
            })
            .ToListAsync(ct);

        return Ok(new PagedResult<UserListItemResponse> { Data = data, TotalRecords = total });
    }

    [HttpDelete("delete-user")]
    public async Task<IActionResult> DeleteUser([FromQuery] string userId)
    {
        var response = await _identity.DeleteUser(userId);
        if (!response.Success)
            return BadRequest(response);
        return Ok(response);
    }

    [HttpPatch("update-password")]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePassword model)
    {
        var response = await _identity.UpdatePasswordAsync(model);
        if (!response.Success)
            return BadRequest(response);
        return Ok(response);
    }

    [HttpPatch("update-name")]
    public async Task<IActionResult> UpdateName([FromBody] PatchUserRequest model)
    {
        var response = await _identity.UpdateNameAsync(model);
        if (!response.Success)
            return BadRequest(response);
        return Ok(response);
    }

    [HttpPut("update-user")]
    public async Task<ActionResult<BaseResponse<UserListItemResponse>>> UpdateUser([FromQuery] string userId, [FromBody] UpdateUserRequest model)
    {
        var response = await _identity.UpdateUserAsync(userId, model);
        if (!response.Success)
            return BadRequest(response);

        var user = await _users.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return Ok(new BaseResponse<UserListItemResponse> { Page = 0, PageSize = 0, TotalRecords = 0 });

        var dto = new UserListItemResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email ?? "",
            Username = user.UserName ?? "",
            Profile = user.Profile,
            ReceiveEmails = user.ReceiveEmails
        };

        return Ok(new BaseResponse<UserListItemResponse> { Data = dto, Page = 0, PageSize = 0, TotalRecords = 0 });
    }

    [HttpPatch("set-received-email")]
    public async Task<IActionResult> SetReceivedEmail([FromQuery] string userId, [FromBody] SetReceiveEmailRequest body)
    {
        var user = await _users.FindByIdAsync(userId);
        if (user == null)
            return NotFound(new { errors = new Dictionary<string, string[]> { ["userId"] = new[] { "Usuário não encontrado." } } });

        user.ReceiveEmails = body.ReceiveEmail;
        var result = await _users.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                errors = new Dictionary<string, string[]>
                {
                    ["user"] = result.Errors.Select(e => e.Description).ToArray()
                }
            });
        }

        return Ok(new { });
    }

    private IQueryable<ApplicationUser> FilterUsers(string? searchInput)
    {
        var q = _users.Users;
        if (!string.IsNullOrWhiteSpace(searchInput))
        {
            var s = searchInput.Trim();
            q = q.Where(u => u.Name.Contains(s) || (u.Email != null && u.Email.Contains(s)) || (u.UserName != null && u.UserName.Contains(s)));
        }

        if (Request.Query.TryGetValue("profile", out var profVals))
        {
            var profiles = profVals.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (profiles.Length > 0)
                q = q.Where(u => profiles.Contains(u.Profile));
        }

        return q;
    }
}
