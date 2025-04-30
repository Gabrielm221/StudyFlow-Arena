using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using StudyFlowArena.API.DTOs;
using StudyFlowArena.API.Interfaces;
using StudyFlowArena.API.Models;
using StudyFlowArena.API.Services;
using System.Text;


namespace StudyFlowArena.API.Controllers{
    [ApiController]
    [Route("api/[controller]")]

    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        public readonly TokenService _tokenService;

        public readonly TokenBlackListService _tokenBlackListService;

        public AuthController(IAuthRepository authRepository, TokenService tokenService, TokenBlackListService tokenBlackListService){
            _authRepository = authRepository;
            _tokenService = tokenService;
            _tokenBlackListService = tokenBlackListService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(UserRegisterDto userRegisterDto)
        {
            if(await _authRepository.UserExists(userRegisterDto.Email))
            return BadRequest("Email already exists");

            var user = new User{
                Username = userRegisterDto.Username,
                Email = userRegisterDto.Email
            };

            var createdUser = await _authRepository.Register(user, userRegisterDto.Password);

            var token = _tokenService.CreateToken(createdUser);

            return Ok(new {Token = token});
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(UserLoginDto userLoginDto){
            var user = await _authRepository.Login(userLoginDto.Email, userLoginDto.Password);

            if(user == null)
                return Unauthorized("Invalid email or password");

            var token = _tokenService.CreateToken(user);

            return Ok(new {Token = token});
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Extract Token from Authorization Header
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
                return BadRequest("No token found in the request");

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                // Token validation params
                var validationParams = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = false, 

                    ValidIssuer = _tokenService.Issuer,
                    ValidAudience = _tokenService.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenService.Secret))
                };

                // Token validation
                tokenHandler.ValidateToken(token, validationParams, out _);
            }
            catch
            {
                return BadRequest("Invalid token format");
            }

            // Read the token, and get the explanation
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var expiry = jwtToken.ValidTo;

            // Add token to blacklist
            _tokenBlackListService.BlackListToken(token, expiry);

            return Ok(new { message = "Logged out successfully. Token blacklisted." });
        }

    }
}