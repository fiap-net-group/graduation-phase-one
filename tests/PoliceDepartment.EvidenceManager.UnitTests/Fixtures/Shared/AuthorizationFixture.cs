using Microsoft.IdentityModel.Tokens;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PoliceDepartment.EvidenceManager.UnitTests.Fixtures.Shared
{
    public class AuthorizationFixture
    {
        public string LoginUrl => "http://localhost/api/fake/authorization/login/";
        public string LogoutUrl => "http://localhost/api/fake/authorization/logout/";

        public AccessTokenViewModel GenerateViewModel(bool valid, Guid? userId = null)
        {
            var id = userId is null ? Guid.NewGuid().ToString() : userId.Value.ToString();
            return valid ?
                new AccessTokenViewModel("Bearer", GenerateFakeJwtToken(), DateTime.Now.AddDays(1), id) :
                new AccessTokenViewModel();
        }

        public string GenerateFakeJwtToken()
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(),
                SigningCredentials =
                    new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())), SecurityAlgorithms.HmacSha256),
                Audience = "audience",
                Issuer = "issuer",
                Expires = DateTime.UtcNow.AddSeconds(300)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
