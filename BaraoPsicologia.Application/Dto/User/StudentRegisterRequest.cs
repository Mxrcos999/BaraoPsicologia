using System.ComponentModel.DataAnnotations;

namespace BaraoPsicologia.Application.Dto.User;


public class AdminRegisterRequest
{
    [Required(ErrorMessage = "O nome deve ser informado")]
    [StringLength(50, ErrorMessage = "O nome deve conter até 50 caracteres.")]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "O nome não deve conter números.")]
    public string Name { get; set; }
    public string Email { get; set; } 
}
public class StudentRegisterRequest
{
    [Required(ErrorMessage = "O nome deve ser informado")]
    [StringLength(50, ErrorMessage = "O nome deve conter até 50 caracteres.")]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "O nome não deve conter números.")]
    public string Name { get; set; }
    public string Email { get; set; } 
    public string StudentCode { get; set; }
    public string Password { get; set; }
    [Compare(nameof(Password), ErrorMessage = "As senhas devem ser iguais.")]
    public string ConfirmPassword { get; set; }
}
