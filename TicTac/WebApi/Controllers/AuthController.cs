using Datasource.Iinterfaces;
using Datasource.Security;
using Di;
using Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTO;
using static System.Net.Mime.MediaTypeNames;

namespace WebApi;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IGameRepository _repo;
    public AuthController()
    {
        _authService = Configuration.AuthService;
        _repo = Configuration.GameRepository;
    }
    [AllowAnonymous]
    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] SignUpRequestDto request)=> 
        _authService.SignUp(request.Login, request.Password)?
        Ok(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{request.Login}:{request.Password}")))
        :
        BadRequest("User is exist or unexpected error");

    [AllowAnonymous]
    [HttpPost("loginin")]
    public async Task<IActionResult> Login([FromBody] JwtRequest request)
    {
        try
        {
            return Ok(_authService.SignIn(request));
        }catch(Exception ex)
        {
            Console.WriteLine($"{ex.Message} || {ex.InnerException}\n {ex.StackTrace}");
            return Unauthorized("Error log in ");
        }

    }
    [AllowAnonymous]
    [HttpPost("refresh_accessToken")]
    public async Task<IActionResult> UpdateAccessToken([FromBody] RefreshJwtRequestDto request)
    {
        try
        {
            var refreshToken = _repo.GetRefreshToken(HashTokens.HashToken(request.RefreshToken));
            return Ok(_authService.UpdateAccessToken(refreshToken));
        }catch(Exception ex)
        {
            Console.WriteLine($"{ex.Message} ||| {ex.InnerException} \n {ex.StackTrace}");
            return Unauthorized();
        }
    }

    [HttpPost("refresh_refreshToken")]
    public async Task<IActionResult> UpdateRefreshToken([FromBody] RefreshJwtRequestDto request)
    {
        try
        {
            var refreshToken = _repo.GetRefreshToken(HashTokens.HashToken(request.RefreshToken));
            return Ok(_authService.UpdateRefreshToken(refreshToken));
        }
        catch (Exception ex)
        {
            return Unauthorized();
        }
    }
}
