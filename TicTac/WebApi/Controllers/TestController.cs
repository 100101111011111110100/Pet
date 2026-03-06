using Microsoft.AspNetCore.Mvc;

using Di;
using Domain.Model;
using Datasource.Iinterfaces;
using Domain.Interfaces;
using Datasource.Security;
using WebApi.DTO;
using System.Security;
namespace WebApi;

[ApiController]
[Route("[controller]")]
public class TestController : Controller
{
    #region  Properties
        private IGameRepository _repo;
        private ITicTacService _service;
    private IPasswordHasher _hasher;
        public TestController()
        {
            _hasher = Configuration.PasswordHasher;
            _repo = Configuration.GameRepository;
            _service = Configuration.GameServices;
        }
    [HttpPut("new_game_test")]
    public IActionResult AddGameTest()
    {
        var game = new Game()
        {
            
        };
       _repo.SetGame(new Game());
       return Ok(); 
    }
    [HttpPut("new_game")]
    public IActionResult AddGame([FromBody] GameDto game)
    {
        _repo.SetGame(game.ConvertToDomainGame());
        return Ok();
    }
    [HttpPut("set_new_user")]
    public IActionResult SetDefaultUser()
    {
        var user = new User();
        Pbkdf2PasswordHasher hasher = new();
        _repo.SetUser(user);
        _repo.SetUserCredential(new UserCredential(user.UUID,"test",hasher.Hash("testPwd",1)));
        return Ok("User \"tes\" added");
    }
    [HttpPut("new_user")]
    public IActionResult AddUser([FromBody] UserDto user)
    {
        _repo.SetUser(user.ConvertToDomainUser());
        return Ok();
    }
    [HttpGet("get_game")]
    public IActionResult GetGame([FromQuery] Guid id)
    {
        var game = new GameDto();
        var dbGame = _repo.GetGame(id);
        game.ConvertFromDomainGame(dbGame);
        return Ok(game);
    }
    [HttpGet("get_field")]
    public IActionResult GetField([FromQuery] Guid id)
    {
        var field = new FieldDto();
        var dbField = _repo.GetField(id);
        field.ConvertFromDomainField(dbField);
        return Ok(field);
    }
    [HttpPost("turn")]
    public IActionResult GetTurn([FromBody] GameDto game)
    {
        _repo.SetGame(game.ConvertToDomainGame());
        return Ok(game);
    }
    [HttpGet("check_cred")]
    public IActionResult SetCredentials([FromQuery] Guid userId)
    {
        var cred = _repo.GetUserCredential(userId);
        return Ok(cred);
    }
    [HttpPut("create_akk")]
    public IActionResult Registration([FromQuery] string Login, [FromQuery] string Password)
    {
        var newUser = new User();
        var creds = new UserCredential(newUser.UUID, Login, _hasher.Hash(Password, 1));
        _repo.SetUserCredential(creds);
        _repo.SetUser(newUser);
        return Ok(creds);
    }
    [HttpGet("sign_in")]
    public IActionResult CheckPassword([FromQuery]string Login, [FromQuery] string Password) {
        var creds = _repo.GetUserCredential(Login);
        return _hasher.Verify(Password,creds.Password)?Ok("Passwords equals"):BadRequest("Passwords not equaled");
    }
    #endregion Properties
}