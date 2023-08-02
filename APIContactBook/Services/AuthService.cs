using ContactBookApi;
using ContactBookApi.Models;
using ContactBookApi.Models.DTOs;
using ContactBookApi.Services.IServices;
using Microsoft.AspNetCore.Identity;
using System;

namespace ContactBookApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly ContactApiContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(ContactApiContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator )
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }
        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = _context.AppUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());

            if (user != null)
            {
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    //create role if it does not exist
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDTO)
        {
            var user = _context.AppUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower());

            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

            if (user == null || isValid == false)
            {
                return new LoginResponseDto()
                {
                    User = null,
                    Token = ""
                };
            }

            //If user is found, generate JWT Token
            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            AppUserDto applicationUserDTO = new()
            {
                Email = user.Email,
                ID = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber
            };

            LoginResponseDto loginResponseDTO = new LoginResponseDto()
            {
                User = applicationUserDTO,
                Token = token
            };

            return loginResponseDTO;
        }

        public async Task<string> Register(RegistrationRequestDto registrationRequestDTO)
        {
            AppUser user = new()
            {
                UserName = registrationRequestDTO.Email,
                Email = registrationRequestDTO.Email,
                NormalizedEmail = registrationRequestDTO.Email.ToUpper(),
                Name = registrationRequestDTO.Name,
                PhoneNumber = registrationRequestDTO.PhoneNumber,
                
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDTO.Password);
                if (result.Succeeded)
                {
                    var userToReturn = _context.AppUsers.First(u => u.UserName == registrationRequestDTO.Email);

                    AppUserDto applicationUserDTO = new()
                    {
                        Email = userToReturn.Email,
                        ID = userToReturn.Id,
                        Name = userToReturn.Name,
                        PhoneNumber = userToReturn.PhoneNumber
                    };
                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            }
            catch (Exception)
            {
            }
            return "Error Encountered";
        }
    }
}
