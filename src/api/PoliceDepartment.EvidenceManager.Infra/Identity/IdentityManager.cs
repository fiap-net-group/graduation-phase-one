using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PoliceDepartment.EvidenceManager.Domain.Authorization;
using PoliceDepartment.EvidenceManager.Domain.Logger;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PoliceDepartment.EvidenceManager.Infra.Identity
{
    [ExcludeFromCodeCoverage]
    public sealed class IdentityManager : IIdentityManager
    {
        private readonly IConfiguration _configuration;
        private readonly ILoggerManager _logger;
        private readonly UserManager<IdentityUser> _userManager;
        public IdentityManager(IConfiguration configuration, 
                               ILoggerManager logger, 
                               UserManager<IdentityUser> userManager)
        {
            _configuration = configuration;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<AccessTokenModel> AuthenticateAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (!await ValidateLoginAsync(user, email, password))
                return new AccessTokenModel();

            var claims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            claims.Add(new Claim(JwtRegisteredClaimNames.UniqueName, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));

            foreach (var userRole in userRoles)
                claims.Add(new Claim("role", userRole));

            _logger.LogDebug("Valid login", ("username", email));

            return GenerateToken(user, new ClaimsIdentity(claims));
        }

        private async Task<bool> ValidateLoginAsync(IdentityUser user, string email, string password)
        {
            if (user is null)
            {
                _logger.LogInformation("User don't exists", ("username", email));

                return false;
            }

            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                _logger.LogInformation("Invalid password", ("username", email));

                return false;
            }

            return true;
        }

        private AccessTokenModel GenerateToken(IdentityUser user, ClaimsIdentity claims)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                SigningCredentials =
                    new SigningCredentials(
                        new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
                Audience = _configuration["Jwt:Audience"],
                Issuer = _configuration["Jwt:Issuer"],
                Expires = DateTime.UtcNow.AddSeconds(300)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AccessTokenModel("Bearer", tokenHandler.WriteToken(token), tokenDescriptor.Expires.Value, user.Id);
        }

        private static long ToUnixEpochDate(DateTime date)
           => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

        public async Task<IdentityResult> CreateAsync(string email, 
                                                      string userName, 
                                                      string password, 
                                                      string officerType){
            var user = new IdentityUser
            {
                UserName = userName,
                Email = email,
                EmailConfirmed = true,
            };

            var result = await _userManager.CreateAsync(user, password);

            if(result.Succeeded)            
                await _userManager.AddClaimAsync(user, new Claim("OfficerType", officerType));
            
            return result;
        }
    
        public async Task<IdentityUser> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IdentityResult> SignOutAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            return await _userManager.UpdateSecurityStampAsync(user);
        }
    }
}
