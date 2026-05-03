using BaraoPsicologia.Application.Dto.Psychology;
using BaraoPsicologia.Application.Dto.Shared;
using BaraoPsicologia.Domain.Entities;
using BaraoPsicologia.Infra.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BaraoPsicologia.API.Controllers;

[ApiController]
[Route("patients")]
public sealed class PatientsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public PatientsController(ApplicationDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<PagedResult<PatientResponse>>> GetList
        ([FromQuery] BaseGetRequest request, CancellationToken ct)
    {
        var (page, pageSize) = ListQueryHelpers.ReadPaging(Request.Query);
        var q = _db.Patients.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.SearchInput))
        {
            var s = request.SearchInput.Trim();
            q = q.Where(p => p.Name.Contains(s) || p.PhoneNumber.Contains(s) || p.ParentName.Contains(s));
        }

        q = q.ApplyCreatedAtFilters(Request.Query);
        var total = await q.CountAsync(ct);
        var data = await q.OrderByDescending(p => p.Id)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(p => new PatientResponse
            {
                Id = p.Id,
                Name = p.Name,
                DateBirth = p.DateBirth,
                PhoneNumber = p.PhoneNumber,
                ParentName = p.ParentName,
                ParentDegree = p.ParentDegree,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .ToListAsync(ct);

        return Ok(new PagedResult<PatientResponse> { Data = data, TotalRecords = total });
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<PatientResponse>> GetById(long id, CancellationToken ct)
    {
        var p = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (p == null)
            return NotFound(new { errors = new Dictionary<string, string[]> { ["id"] = new[] { "Paciente não encontrado." } } });

        return Ok(ToResponse(p));
    }

    [HttpPost]
    public async Task<ActionResult<PatientResponse>> Create
        ([FromBody] CreatePatientRequest body, CancellationToken ct)
    {
        var entity = new Patient
        {
            Name = body.Name,
            DateBirth = body.DateBirth,
            PhoneNumber = body.PhoneNumber,
            ParentName = body.ParentName,
            ParentDegree = body.ParentDegree,
            CreatedAt = DateTime.UtcNow
        };
        _db.Patients.Add(entity);
        await _db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, ToResponse(entity));
    }

    [HttpPatch("{id:long}")]
    public async Task<ActionResult<PatientResponse>> Patch(long id, [FromBody] PatchPatientRequest body, CancellationToken ct)
    {
        var entity = await _db.Patients.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null)
            return NotFound(new { errors = new Dictionary<string, string[]> { ["id"] = new[] { "Paciente não encontrado." } } });

        if (body.Name != null) entity.Name = body.Name;
        if (body.DateBirth.HasValue) entity.DateBirth = body.DateBirth.Value;
        if (body.PhoneNumber != null) entity.PhoneNumber = body.PhoneNumber;
        if (body.ParentName != null) entity.ParentName = body.ParentName;
        if (body.ParentDegree != null) entity.ParentDegree = body.ParentDegree;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return Ok(ToResponse(entity));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var entity = await _db.Patients.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null)
            return NotFound();

        _db.Patients.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    private static PatientResponse ToResponse(Patient p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        DateBirth = p.DateBirth,
        PhoneNumber = p.PhoneNumber,
        ParentName = p.ParentName,
        ParentDegree = p.ParentDegree,
        CreatedAt = p.CreatedAt,
        UpdatedAt = p.UpdatedAt
    };
}
