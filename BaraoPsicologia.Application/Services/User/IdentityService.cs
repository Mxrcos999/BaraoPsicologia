using BaraoPsicologia.Application.Dto.Shared;
using BaraoPsicologia.Application.Dto.User;
using BaraoPsicologia.Application.Interfaces.Services;
using BaraoPsicologia.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;

public class IdentityService : IIdentityService
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEmailService _emailSender;
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
    private readonly JwtOptions _jwtOptions;

    public IdentityService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IOptions<JwtOptions> jwtOptions, IHttpContextAccessor httpContextAccessor, IEmailService emailSender, IPasswordHasher<ApplicationUser> passwordHasher)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _jwtOptions = jwtOptions.Value;
        _httpContextAccessor = httpContextAccessor;
        _emailSender = emailSender;
        _passwordHasher = passwordHasher;
    }

    public async Task<DefaultResponse> UpdateNameAsync(PatchUserRequest model)
    {
        var response = new DefaultResponse();

        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null)
        {
            response.Errors.AddError("Usuário não encontrado.");
            return response;
        }

        user.Name = model.Name;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            response.Errors.AddError("Erro ao alterar nome do usuário");

        return response;
    }
    public async Task<DefaultResponse> UpdateUserAsync(string userId, UpdateUserRequest model)
    {
        var response = new DefaultResponse();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            response.Errors.AddError("Usuário não encontrado.");
            return response;
        }

        user.Name = model.Name;
        user.Email = model.Email;
        user.Profile = model.Profile;
        user.ReceiveEmails = model.ReceiveEmails;

        await _userManager.SetUserNameAsync(user, model.Username);
        await _userManager.SetEmailAsync(user, model.Email);

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            response.Errors.AddError("Erro ao alterar usuário");

        return response;
    }
    public async Task<UserLoginResponse> LoginAsync(UserLoginRequest userLogin)
    {
        SignInResult signInResult = await _signInManager.PasswordSignInAsync(userLogin.UserName, userLogin.Password, isPersistent: false, lockoutOnFailure: true);

        if (signInResult.Succeeded)
        {
            var credenciais = await GenerateCredentials(userLogin.UserName);
            return credenciais;
        }

        UserLoginResponse userLoginResponse = new UserLoginResponse(signInResult.Succeeded);
        if (!signInResult.Succeeded)
        {
            if (signInResult.IsLockedOut)
            {
                userLoginResponse.Errors.AddError("Esta conta está bloqueada.");
            }
            else if (signInResult.IsNotAllowed)
            {
                userLoginResponse.Errors.AddError("Esta conta não tem permissão para entrar.");
            }
            else if (signInResult.RequiresTwoFactor)
            {
                userLoginResponse.Errors.AddError("Confirme seu email.");
            }
            else
            {
                userLoginResponse.Errors.AddError("Nome de usuário ou senha estão incorretos.");
            }
        }

        return userLoginResponse;
    }
    public async Task<UserRegisterResponse> RegisterAdminAsync(string profile, AdminRegisterRequest request)
    {
        var user = new ApplicationUser()
        {
            Email = request.Email,
            Profile = profile,
            Name = request.Name,
            UserName = request.Email,
        };

        user.EmailConfirmed = true;

        var password = GeneratePassword();
        IdentityResult result = await _userManager.CreateAsync(user, password);

        var response = await ValidateRegisterAsync(result, user.Email);

        if (!response.Success)
            return response;

        response.Data = password;
        await _emailSender.SendPassword(user.Email, user.Name, password);

        return response;
    }
    public async Task<DefaultResponse> DeleteUser(string id)
    {
        var response = new DefaultResponse();
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            response.Errors.AddError("Usuário não encontrado.");
            return response;
        }

        await _userManager.DeleteAsync(user);
        return new();
    }
    public async Task<UserRegisterResponse> RegisterStudentAsync(string profile, StudentRegisterRequest userRegister)
    {
        string email = userRegister.StudentCode + "@baraodemaua.edu.br";
        var user = new ApplicationUser()
        {
            Email = email,
            Profile = profile, 
            Name = userRegister.Name,
            UserName = userRegister.StudentCode,
        };

        IdentityResult result = await _userManager.CreateAsync(user, userRegister.Password);

        if (result.Succeeded)
            await SendConfirmMail(email);

        return await ValidateRegisterAsync(result, email);
    }

    public async Task<DefaultResponse> UpdatePasswordAsync(UpdatePassword dto)
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        var user = await _userManager.FindByIdAsync(userId);
        var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.Password);

        return new DefaultResponse();
    }

    private async Task<UserRegisterResponse> ValidateRegisterAsync(IdentityResult result, string email)
    {

        UserRegisterResponse userRegisterResponse = new UserRegisterResponse(result.Succeeded);

        if (!result.Succeeded)
        {
            foreach (var erroAtual in result.Errors)
            {
                switch (erroAtual.Code)
                {
                    case "PasswordRequiresNonAlphanumeric":
                        userRegisterResponse.Errors.AddError("A senha precisa conter pelo menos um caracter especial - ex( * | ! ).");
                        break;

                    case "PasswordRequiresDigit":
                        userRegisterResponse.Errors.AddError("A senha precisa conter pelo menos um número (0 - 9).");
                        break;

                    case "PasswordRequiresUpper":
                        userRegisterResponse.Errors.AddError("A senha precisa conter pelo menos um caracter em maiúsculo.");
                        break;

                    case "DuplicateUserName":
                        userRegisterResponse.Errors.AddError("O email informado já foi cadastrado! Verifique seu email institucional para ativar a conta.");
                        await SendConfirmMail(email);

                        break;

                    default:
                        userRegisterResponse.Errors.AddError("Erro ao criar usuário.");
                        break;
                }

            }
        }

        return userRegisterResponse;
    }
    public async Task<IEnumerable<ApplicationUser>> GetUser()
    {
        var result = _userManager.Users.AsEnumerable();

        return result;

    }

    protected async Task<UserLoginResponse> GenerateCredentials(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        var accessTokenClaims = await GetClaims(user, adicionarClaimsUsuario: true);
        var refreshTokenClaims = await GetClaims(user, adicionarClaimsUsuario: false);

        var dataExpiracaoAccessToken = DateTime.Now.AddSeconds(_jwtOptions.AccessTokenExpiration);
        var dataExpiracaoRefreshToken = DateTime.Now.AddSeconds(10800);

        var accessToken = GenerateToken(accessTokenClaims, dataExpiracaoAccessToken);
        var refreshToken = GenerateToken(refreshTokenClaims, dataExpiracaoRefreshToken);
        return new UserLoginResponse
        (
            true,
            user.Profile,
            accessToken,
            refreshToken,
            _jwtOptions.RefreshTokenExpiration,
            _jwtOptions.AccessTokenExpiration,
            user.Name,
            user.Id,
            user.Email
        );
    }

    protected string GenerateToken(IEnumerable<Claim> claims, DateTime dataExpiracao)
    {
        JwtSecurityToken token = new JwtSecurityToken(_jwtOptions.Issuer, _jwtOptions.Audience, claims, DateTime.Now, expires: dataExpiracao, signingCredentials: _jwtOptions.SigningCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    private async Task<IList<Claim>> GetClaims(ApplicationUser user, bool adicionarClaimsUsuario)
    {
        var claims = await _userManager.GetClaimsAsync(user);

        var roles = await _userManager.GetRolesAsync(user);

        claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
        claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

        var now = DateTimeOffset.UtcNow;
        claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, now.ToUnixTimeSeconds().ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64));

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims;
    }

    public static string GeneratePassword()
    {
        var Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        char[] password = new char[6];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            byte[] randomBytes = new byte[6];
            rng.GetBytes(randomBytes);

            for (int i = 0; i < password.Length; i++)
            {
                int index = randomBytes[i] % Characters.Length;
                password[i] = Characters[index];
            }
        }
        return new string(password);
    }

    public async Task<string> SendConfirmMail(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return null;

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var encodedToken = WebUtility.UrlEncode(token);

        var confirmationLink = $"https://baraotickets-dxcafcc6h9h0aga6.eastus2-01.azurewebsites.net/user/ConfirmEmail?userId={user.Id}&token={encodedToken}";
        await _emailSender.SendConfirmMail(email, user.Name, confirmationLink);
        return confirmationLink;
    }

    public async Task<bool> UnlockUser(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);

        token = WebUtility.UrlDecode(token);
        user.EmailConfirmed = true;

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
            return true;

        return false;
    }

    public async Task<DefaultResponse> ForgotPassword(ForgotPasswordRequest model)
    {
        var response = new DefaultResponse();
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null)
        {
            response.Errors.AddError("Usuário não encontrado.");
            return response;
        }

        var novaSenha = GeneratePassword();

        var hash = _passwordHasher.HashPassword(user, novaSenha);
        user.PasswordHash = hash;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                response.Errors.AddError(error.Description);
            return response;
        }
        await _emailSender.SendForgotPasswordEmail(model.Email, user.Name, novaSenha);

        return response;
    }
    private string GeneratePassword(int length = 10)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%&";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    } 


}