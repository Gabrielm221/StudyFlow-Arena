using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using StudyFlowArena.API.Models;

namespace StudyFlowArena.API.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;
        private readonly string _jwtSecret;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;
        private readonly int _jwtExpiryMinutes;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
            _jwtSecret = configuration["Jwt:SecretKey"] ?? throw new ArgumentNullException("Jwt:SecretKey");
            _jwtIssuer = configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer");
            _jwtAudience = configuration["Jwt:Audience"] ?? throw new ArgumentNullException("Jwt:Audience");
            _jwtExpiryMinutes = int.Parse(configuration["Jwt:ExpiryInMinutes"] ?? "60");
        }

        // Expose JWT settings to be used for token validation
        public string Secret => _jwtSecret;
        public string Issuer => _jwtIssuer;
        public string Audience => _jwtAudience;

        public string CreateToken(User user)
        {
            // Create the JWT claims
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // Create signing credentials with the secret key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create the token descriptor with issuer, audience, claims and expiration
            var tokenDescriptor = new JwtSecurityToken(
                issuer: _jwtIssuer,
                audience: _jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtExpiryMinutes),
                signingCredentials: creds
            );

            // Generate the token
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(tokenDescriptor);
        }
    }
}
