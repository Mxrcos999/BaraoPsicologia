namespace BaraoPsicologia.Domain.Interfaces.Services;

public interface IEmailService
{
    Task<bool> SendConfirmMail(string mail, string name, string link);
    Task<bool> SendForgotPasswordEmail(string email, string userName, string senha);
    Task<bool> SendPassword(string mail, string name, string password);
}
