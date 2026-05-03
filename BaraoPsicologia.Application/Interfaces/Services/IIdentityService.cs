using BaraoPsicologia.Application.Dto.Shared;
using BaraoPsicologia.Application.Dto.User;
using BaraoPsicologia.Domain;

namespace BaraoPsicologia.Application.Interfaces.Services;

public interface IIdentityService
{
    Task<DefaultResponse> UpdateNameAsync(PatchUserRequest model);
    Task<DefaultResponse> DeleteUser(string id);
    Task<DefaultResponse> UpdateUserAsync(string userId, UpdateUserRequest model);
    Task<bool> UnlockUser(string userId, string token);
    Task<DefaultResponse> UpdatePasswordAsync(UpdatePassword dto);
    Task<DefaultResponse> ForgotPassword(ForgotPasswordRequest model);
    Task<UserLoginResponse> LoginAsync(UserLoginRequest userLogin);
    Task<UserRegisterResponse> RegisterAdminAsync(string type, AdminRegisterRequest request);
    Task<UserRegisterResponse> RegisterStudentAsync(string type, StudentRegisterRequest userRegister);
}
