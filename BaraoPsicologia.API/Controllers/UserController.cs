using BaraoPsicologia.Application.Dto.Shared;
using BaraoPsicologia.Application.Dto.User;
using BaraoPsicologia.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
namespace BaraoPsicologia.API.Controllers
{

    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IIdentityService _userService;
        public UserController(IIdentityService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("user/post-student-user")]
        public async Task<IActionResult> RegisterStudentAsync(StudentRegisterRequest request)
        {
            var response = await _userService.RegisterStudentAsync("student", request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpDelete]
        [Route("user/delete-user")]
        public async Task<IActionResult> DeleteUserAsync(string userId)
        {
            var response = await _userService.DeleteUser(userId);

            if (!response.Sucess)
                return BadRequest(response);

            return Ok(response);
        }

        //[HttpGet]
        //[Route("user/get-admin-list")]
        //public async Task<IActionResult> GetUsersAsync()
        //{
        //    var response = await _userService.GetUsers();

        //    return Ok(response);
        //}

        [HttpPatch]
        [Route("user/update-password")]
        public async Task<IActionResult> UpdateChangeAsync(UpdatePassword model)
        {
            var response = await _userService.UpdatePasswordAsync(model);

            if (!response.Sucess)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost]
        [Route("user/post-admin-user")]
        public async Task<IActionResult> RegisterAdminAsync(AdminRegisterRequest request)
        {
            var response = await _userService.RegisterAdminAsync("admin", request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost]
        [Route("user/user-login")]
        public async Task<IActionResult> LoginUserAsync(UserLoginRequest request)
        {
            var response = await _userService.LoginAsync(request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        //[HttpPatch]
        //[Route("user/set-received-email")]
        //public async Task<IActionResult> ReceivedEmailAsync([FromQuery] string userId, [FromBody] AdminFlagUpdate model)
        //{
        //    var response = await _userService.SetReceiveEmailAsync(userId, model.ReceiveEmail);
        //    var defaultResponse = new DefaultResponse();
        //    if (!response)
        //    {
        //        defaultResponse.Errors.AddError("Erro ao atualizar flag");
        //        return BadRequest(defaultResponse);

        //    }

        //    return Ok(defaultResponse);
        //}

        [HttpPatch]
        [Route("user/update-name")]
        public async Task<IActionResult> UpdateNameAsync(PatchUserRequest model)
        {
            var response = await _userService.UpdateNameAsync(model);

            if (!response.Sucess)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPut]
        [Route("user/update-user")]
        public async Task<IActionResult> UpdateUserAsync
            ([FromQuery] string userId,
            [FromBody] UpdateUserRequest model)
        {
            var response = await _userService.UpdateUserAsync(userId, model);

            if (!response.Sucess)
                return BadRequest(response);

            return Ok(response);
        }
        [HttpGet("/user/ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var result = await _userService.UnlockUser(userId, token);

            if (result)
                return Redirect("https://feedback.baraodemaua.net/login");

            return BadRequest("Token expirado!");
        }

        //[HttpPost("/user/forgot-password")]
        //public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        //{
        //    var response = await _userService.ForgotPassword(model);

        //    if (!response.Sucess)
        //        return BadRequest(response);

        //    return Ok(response);
        //}
    }

}
