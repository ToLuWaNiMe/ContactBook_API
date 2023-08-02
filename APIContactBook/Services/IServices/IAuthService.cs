using ContactBookApi.Models.DTOs;

namespace ContactBookApi.Services.IServices
{
    public interface IAuthService
    {
        Task<string> Register(RegistrationRequestDto registrationRequestDTO);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDTO);
        Task<bool> AssignRole(string email, string roleName);
    }
}
