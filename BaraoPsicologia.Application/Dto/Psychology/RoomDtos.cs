namespace BaraoPsicologia.Application.Dto.Psychology;

public sealed class RoomResponse
{
    public long Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public long ClinicId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public sealed class CreateRoomRequest
{
    public string Number { get; set; } = string.Empty;
    public long ClinicId { get; set; }
}

public sealed class PatchRoomRequest
{
    public string? Number { get; set; }
    public long? ClinicId { get; set; }
}
