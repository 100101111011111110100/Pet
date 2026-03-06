using Microsoft.AspNetCore.Mvc;

using Datasource.Iinterfaces;
using Di;
using Domain.Model;
using WebApi.DTO;
using Domain.Interfaces;
namespace WebApi;

[ApiController]
[Route("[controller]")]
public class UserController : Controller
{
    private IGameRepository _repo;
    private IJwtProvider _jwtProvider;
    public UserController()
    {
        _repo = Configuration.GameRepository;
        _jwtProvider = Configuration.JwtProvider;
    }
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserInfo(Guid userId)
    {
        User? user = null;
        try
        {
            user = _repo.GetUser(userId);
        }
        catch { return BadRequest("User is not found"); }
        var resultUser = new UserDto(user);
        return Ok(resultUser);
    }
    [HttpGet("info")]
    public async Task<IActionResult> GetUserInfo()
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        try
        {
            return Ok(new UserDto(_repo.GetUser(userId)));
        }
        catch (Exception)
        {
            return BadRequest("User not found");
        }
    }
}