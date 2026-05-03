using BaraoPsicologia.Application.Dto.Shared;
using BaraoPsicologia.Application.Dto.User;
using BaraoPsicologia.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BaraoPsicologia.API.Controllers;

[ApiController]
[Route("user")]
public sealed class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _users;

    public UsersController(UserManager<ApplicationUser> users) => _users = users;

    [HttpGet("options")]
    public async Task<ActionResult<PagedResult<UserOptionResponse>>> GetOptions([FromQuery] string? searchInput, CancellationToken ct)
    {
        var (page, pageSize) = ListQueryHelpers.ReadPaging(Request.Query);
        var q = FilterUsersQuery(searchInput);
        q = q.AsNoTracking();

        var total = await q.CountAsync(ct);
        var data = await q.OrderBy(u => u.Name)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(u => new UserOptionResponse
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email ?? ""
            })
            .ToListAsync(ct);

        return Ok(new PagedResult<UserOptionResponse> { Data = data, TotalRecords = total });
    }

    private IQueryable<ApplicationUser> FilterUsersQuery(string? searchInput)
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
