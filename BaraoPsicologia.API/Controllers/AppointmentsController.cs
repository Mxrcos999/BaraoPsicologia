using BaraoPsicologia.Application.Dto.Psychology;
using BaraoPsicologia.Application.Dto.Shared;
using BaraoPsicologia.Domain.Entities;
using BaraoPsicologia.Domain.Enums;
using BaraoPsicologia.Infra.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BaraoPsicologia.API.Controllers;

[ApiController]
[Route("appointments")]
public sealed class AppointmentsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public AppointmentsController(ApplicationDbContext db) => _db = db;
     
    [HttpGet]
    public async Task<ActionResult<PagedResult<AppointmentResponse>>> GetList
        ([FromQuery] BaseGetRequest request, CancellationToken ct)
    {
        try
        {
            var (page, pageSize) = ListQueryHelpers.ReadPaging(Request.Query);
            var q = _db.Appointments.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(request.SearchInput))
            {
                var s = request.SearchInput.Trim();
                q = q.Where(a => a.UserId.Contains(s));
            }

            q = q.ApplyCreatedAtFilters(Request.Query);
            var total = await q.CountAsync(ct);
            var data = await q.OrderByDescending(a => a.Id)
                .Skip((page - 1) * pageSize).Take(pageSize)
                .Select(a => new AppointmentResponse
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    UserId = a.UserId,
                    AppointmentDate = a.AppointmentDate,
                    RoomId = a.RoomId,
                    Status = a.Status,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt
                })
                .ToListAsync(ct);

            return Ok(new PagedResult<AppointmentResponse> { Data = data, TotalRecords = total });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
            throw ex;
        }

    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<AppointmentResponse>> GetById(long id, CancellationToken ct)
    {
        var a = await _db.Appointments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (a == null)
            return NotFound(new { errors = new Dictionary<string, string[]> { ["id"] = new[] { "Consulta não encontrada." } } });

        return Ok(ToResponse(a));
    }

    [HttpPost]
    public async Task<ActionResult<AppointmentResponse>> Create([FromBody] CreateAppointmentRequest body, CancellationToken ct)
    {
        var entity = new Appointment
        {
            PatientId = body.PatientId,
            UserId = body.UserId,
            RoomId = body.RoomId,
            AppointmentDate = body.AppointmentDate,
            Status = body.Status,
            CreatedAt = DateTime.UtcNow
        };
        _db.Appointments.Add(entity);
        await _db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, ToResponse(entity));
    }

    [HttpPatch("{id:long}")]
    public async Task<ActionResult<AppointmentResponse>> Patch(long id, [FromBody] PatchAppointmentRequest body, CancellationToken ct)
    {
        var entity = await _db.Appointments.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null)
            return NotFound(new { errors = new Dictionary<string, string[]> { ["id"] = new[] { "Consulta não encontrada." } } });

        if (body.PatientId.HasValue) entity.PatientId = body.PatientId.Value;
        if (body.UserId != null) entity.UserId = body.UserId;
        if (body.RoomId.HasValue) entity.RoomId = body.RoomId.Value;
        if (body.AppointmentDate.HasValue) entity.AppointmentDate = body.AppointmentDate.Value;
        if (body.Status.HasValue) entity.Status = body.Status.Value;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return Ok(ToResponse(entity));
    }

    [HttpPatch("{id:long}/status")]
    public async Task<IActionResult> PatchStatus(long id, [FromBody] PatchAppointmentStatusRequest body, CancellationToken ct)
    {
        if (!Enum.IsDefined(typeof(AppointmentStatus), body.Status))
        {
            return BadRequest(new { errors = new Dictionary<string, string[]> { ["status"] = new[] { "Status inválido." } } });
        }

        var entity = await _db.Appointments.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null)
            return NotFound(new { errors = new Dictionary<string, string[]> { ["id"] = new[] { "Consulta não encontrada." } } });

        entity.Status = body.Status;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var entity = await _db.Appointments.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null)
            return NotFound();

        _db.Appointments.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    private static AppointmentResponse ToResponse(Appointment a) => new()
    {
        Id = a.Id,
        PatientId = a.PatientId,
        UserId = a.UserId,
        AppointmentDate = a.AppointmentDate,
        RoomId = a.RoomId,
        Status = a.Status,
        CreatedAt = a.CreatedAt,
        UpdatedAt = a.UpdatedAt
    };
}
