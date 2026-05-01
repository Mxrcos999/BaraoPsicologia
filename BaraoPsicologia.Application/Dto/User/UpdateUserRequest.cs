namespace BaraoPsicologia.Application.Dto.User;

public class PatchUserRequest
{
    public string UserId { get; set; }
    public string Name { get; set; }
}

public class UpdateUserRequest
{
    public string Name { get; set; }
    public string Email { get; set; }
}
