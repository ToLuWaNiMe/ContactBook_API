using ContactBookApi.Models;
using ContactBookApi.Services.IServices;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ContactBookApi.Services
{
    public class JwtTokenGeneratorService : IJwtTokenGenerator
    {
        private readonly JwtOptions _jwtOptions;

        public JwtTokenGeneratorService(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }
        public string GenerateToken(AppUser appUser, IEnumerable<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();



            var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret);



            var claimList = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, appUser.Email),
                new Claim(JwtRegisteredClaimNames.Sub, appUser.Id),
                new Claim(JwtRegisteredClaimNames.Name, appUser.UserName)
            };



            claimList.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));



            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _jwtOptions.Audience,
                Issuer = _jwtOptions.Issuer,
                Subject = new ClaimsIdentity(claimList),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };



            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
