 using System.IdentityModel.Tokens.Jwt;
 using System.Security.Claims;
 using System.Text;
 using Microsoft.IdentityModel.Tokens;
 using StudyFlowArena.API.Models;

 namespace StudyFlowArena.API.Services{
    public class TokenService{

        _configuration = configuration

        private readonly IConfiguration _configuration;
        private readonly string _jwtSecret;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;

        private readonly string _jwtExpiryMinutes;
    }

    public TokenService(IConfiguration configuration){
        _configuration = configuration;
        _jwtSecret = configuration["Jwt:SecretKey"];
        _jwtIssuer = configuration["Jwt:Issuer"];
        _jwtAudience = configuration["Jwt:Audience"];
        _jwtExpiryMinutes = int.Parse(configuration["Jwt:ExpiryInMinutes"]);
    }

    public string CreateToken(User user){
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.getID()),
            new Claim(ClaimTypes.Name, user.getUsername()),
            new Claim(ClaimTypes.Email, user.getEmail())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new JwtSecurityToken(
            issuer : _jwtIssuer,
            audience : _jwtAudience,
            claims : claims,
            expires : DateTime.UtcNow.AddMinutes(_jwtExpiryMinutes),
            signingCredentials = creds;
        )

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(tokenDescriptor);
    }
 }
