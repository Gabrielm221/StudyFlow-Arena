using Microsoft.AspNetCore.Mvc;
using StudyFlowArena.API.DTOs;
using StudyFlowArena.API.Interfaces;
using StudyFlowArena.API.Models;
using StudyFlowArena.API.Services;

namespace StudyFlowArena.API.Controllers{
    [ApiController]
    [Route("api/[controller]")]

    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        public readonly TokenService _tokenService;

        public AuthController(IAuthRepository authRepository, TokenService tokenService){
            _authRepository = authRepository;
            _tokenService = tokenService;
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
    }
}