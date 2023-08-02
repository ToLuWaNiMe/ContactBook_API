using ContactBookApi.Models.DTOs;
using ContactBookApi.Services.IServices;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ContactBookApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        protected ResponseDto response;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
            response = new ResponseDto();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {
            var errorMessage = await _authService.Register(model);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                response.IsSuccess = false;
                response.Message = errorMessage;
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var loginResponse = await _authService.Login(model);

            if (loginResponse.User == null)
            {
                response.IsSuccess = false;
                response.Message = "Username or Password is incorrect";
                return BadRequest(response);
            }
            response.Result = loginResponse;
            return Ok(response);          
        }

        [HttpPost("Assign-role")]
        public async Task<IActionResult> GiveRole([FromBody] RegistrationRequestDto model)
        {
            var assignRoleSuccessful = await _authService.AssignRole(model.Email, model.RoleName.ToUpper());

            if (!assignRoleSuccessful)
            {
                response.IsSuccess = false;
                response.Message = "Error encountered";
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
