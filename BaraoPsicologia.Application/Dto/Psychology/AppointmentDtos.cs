namespace BaraoPsicologia.Application.Dto.Psychology;

public sealed class AppointmentResponse
{
    public long Id { get; set; }
    public long PatientId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
    public long RoomId { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public sealed class CreateAppointmentRequest
{
    public long PatientId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public long RoomId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public int Status { get; set; }
}

public sealed class PatchAppointmentRequest
{
    public long? PatientId { get; set; }
    public string? UserId { get; set; }
    public long? RoomId { get; set; }
    public DateTime? AppointmentDate { get; set; }
    public int? Status { get; set; }
}

public sealed class PatchAppointmentStatusRequest
{
    public int Status { get; set; }
}
