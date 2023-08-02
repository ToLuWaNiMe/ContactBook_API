using ContactBookApi.Models;

namespace ContactBookApi.Services.IServices
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(AppUser applicationUser, IEnumerable<string> roles);
    }
}
