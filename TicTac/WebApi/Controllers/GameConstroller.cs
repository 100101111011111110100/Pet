//using System.Reflection;
using Datasource.Iinterfaces;
using Di;
using Domain.Interfaces;
using Domain.Model;
using Microsoft.AspNetCore.Mvc;
using System.Security;
using System.Security.Claims;
using WebApi.DTO;
namespace WebApi;

[ApiController]
[Route("[controller]")]
public class GameController : Controller
{
    private IGameRepository _repo;
    private ITicTacService _service;
    private IRandomGenerator _randomGenerator;
    public GameController()
    {
        _repo = Configuration.GameRepository;
        _service = Configuration.GameServices;
        _randomGenerator = Configuration.RandomGenerator;
    }
    #region Methods.Public
    [HttpGet("get_game_info/{userId}")]
    public async Task<IActionResult> GetCurentGameByUser(Guid userId)
    {
        User? user = null;
        try
        {
            user = _repo.GetUser(userId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.Message}||{ex.InnerException}");
            return BadRequest("User is not registred");
        }
        if (user.GameId.Equals(Guid.Empty)) return BadRequest("User not in game");
        Game? game = null;
        try
        {
            game = _repo.GetGame(user.GameId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.Message}||{ex.InnerException}");
            return BadRequest("Game not found");
        }
        if (!game.State.Equals(State.PlayerOTurn) && !game.State.Equals(State.PlayersXTurn) ) return BadRequest("Game is ended");
        user.Field = game.Field;

        return Ok(new UserDto(user));
    }
    [HttpPut("create_new_game")]
    public async Task<IActionResult> CreateNewGame(CreateNewGame newGame) => newGame.Opponents switch
    {
        Opponents.UserVsAi => CreateNewGameWithAi(newGame.UserId),
        Opponents.UserVsUser => CreateNewGameWithUser(newGame.UserId),
        _ => BadRequest("Unhandled error")
    };
    [HttpPost("join_to_game/{gameId}")]
    public async Task<IActionResult> ConnectedUserToUserGame(Guid gameId, [FromBody] Guid userId)
    {
        Game? game = null;
        try
        {
            game = _repo.GetGame(gameId);
        } catch (Exception ex)
        {
            return BadRequest("Game not found");
        }
        return ConnectedToGame(game,userId);
    }
    [HttpPost("join_to_random_game")]
    public async Task<IActionResult> ConnectedUserToUserGame([FromQuery] Guid userId)
    {
        Game? game = null;
        try
        {
            game = _repo.GetEmptyUserToUserGame();
        }
        catch (Exception ex)
        {
            return BadRequest("Game not found");
        }
        return ConnectedToGame(game, userId);
    }
    [HttpPost("{gameId}")]
    public async Task<IActionResult> PostGame(Guid gameId, [FromBody] UserDto userDto)
    {
        Game? game = null;
        try
        {
            game = _repo.GetGame(gameId);
        }
        catch (Exception ex)
        {
            return BadRequest("Game not found");
        }
        if (!game.State.Equals(State.PlayersXTurn) && !game.State.Equals(State.PlayerOTurn)) return BadRequest("Game ended or not started");
        User? user = null;
        try
        {
            user = _repo.GetUser(userDto.UUID);
        }
        catch (Exception ex)
        {
            return BadRequest("User not found");
        }
        if (!game.XPlayer.Equals(user.UUID) && !game.OPlayer.Equals(user.UUID)) return BadRequest("User not in this game");
        user.Field = userDto.UserField.ConvertToDomainField();
        if (!_service.ValidationField(game, user)) return BadRequest("Field not accepted");
        switch (game.Opponents)
        {
            case Opponents.UserVsUser:
                return PostGameWithUser(game,userDto);
            case Opponents.UserVsAi:
                return await PostGameWithAi(game, userDto);
            default:
                break;
        }
        return BadRequest("Unhandle exception");
    }
    [HttpGet("finished")]
    public async Task<IActionResult> GetFinishedGameByUser()
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        try
        {
            var list = _repo.GetFinishedGamesByUser(userId);
            var newlist = new List<GameDto>();
            list.ForEach(l => newlist.Add(new GameDto(l)));
            return Ok(newlist);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.Message} {ex.InnerException}\n{ex.StackTrace}");
            return BadRequest();
        }
    }
    
    [HttpGet("avail_cur_games")]
    public async Task<IActionResult> AvailableCurentGames()
    {
        List<Game>? games = null;
        try
        {
            games = _repo.GetEmptyUserToUserGamesList();
        }
        catch (Exception ex)
        {
            return BadRequest("Can't find any empty games");
        }
        List<Guid> emptyGameGuids = new List<Guid>(games.Count);
        for (int i = 0; i < emptyGameGuids.Count; i++)
        {
            emptyGameGuids[i] = games[i].UUID;
        }
        return Ok(emptyGameGuids);
    }
    [HttpGet("leaderboard/{count}")]
    public async Task<IActionResult> GetLeaderBoard(int count)
    {
        try
        {
            var list =_repo.GetNPlayersWithHighKpd(count);
            List<LeaderBoardUserDto> resList = [];
            list.ForEach(k =>
            {
                var user = _repo.GetUser(k.UserId);
                var uCreds = _repo.GetUserCredential(k.UserId);
                resList.Add(new LeaderBoardUserDto()
                {
                    UserId = user.UUID,
                    Login = uCreds.Login
                });
            });
            return Ok(resList);
        }
        catch (Exception)
        {
            return BadRequest("Get leaderBoard error");
        }
    }
    #endregion Methods.Public
    #region Methods.Private
    private IActionResult EndGame(Game game)
    {
        if (game.State.Equals(State.Draw)) return Ok("Game ended , draw !");
        string userVsUserCongStr = $"User : {(game.State.Equals(State.PlayersXVictory)?game.XPlayer:game.OPlayer)}";
        string userVsAiCongStr = $"{(game.State.Equals(State.PlayersXVictory) ? "User" :"Ai")}";
        game.SetGameEnded(true);
        _repo.SetGame(game);
        return Ok($"Congratulations : {(game.Opponents.Equals(Opponents.UserVsUser) ? userVsUserCongStr : userVsAiCongStr)}");
    }
    private IActionResult CreateNewGameWithUser(Guid userId)
    {
        User? user = null;
        try
        {
            user = _repo.GetUser(userId);
        }
        catch (Exception ex)
        {
            return BadRequest("User not found");
        }
        if (CheckPrevGameIsEnded(user.GameId)) return BadRequest($"Previous game for user {user.UUID} is not finished.\nPlease finish previous game and then start new game");
        Game game = new Game();
        game.Time = DateTime.UtcNow;
        user.GameId = game.UUID;
        game.XPlayer = user.UUID;
        game.Opponents = Opponents.UserVsUser;
        game.State = State.WaitingForThePlayers;
        try
        {
            _repo.SetUser(user);
        }
        catch (Exception ex)
        {
            return BadRequest("Can't save user please,contact support");
        }
        try
        {
            _repo.SetGame(game);
        }
        catch (Exception ex)
        {
            return BadRequest("Can't save game please,contact support");
        }
        return Ok($"Game created.Wait second user\nGame id : {game.UUID}");
    }
    private IActionResult CreateNewGameWithAi(Guid userId)
    {
        User? user = null;
        try
        {
            user = _repo.GetUser(userId);
        }
        catch (Exception ex)
        {
            return BadRequest("User not found");
        }
        if (CheckPrevGameIsEnded(user.GameId)) return BadRequest($"Previous game for user {user.UUID} is not finished.\nPlease finish previous game and then start new game");
        Game game = new Game();
        game.Time = DateTime.UtcNow;
        user.GameId = game.UUID;
        user.Symbol = UserSymbols.PlayerSymbol;
        game.XPlayer = user.UUID;
        game.OPlayer = Guid.Empty;
        game.Opponents = Opponents.UserVsAi;
        game.State = State.PlayersXTurn;
        try
        {
            _repo.SetUser(user);
        }
        catch (Exception ex)
        {
            return BadRequest("Can't save user please,contact support");
        }
        try
        {
            _repo.SetGame(game);
        }
        catch (Exception ex)
        {
            return BadRequest("Can't save game please,contact support");
        }
        FieldDto fieldDto = new FieldDto();
        fieldDto.ConvertFromDomainField(game.Field);
        UserDto userDto = new UserDto(user);
        return Ok(userDto);
    }
    private bool CheckPrevGameIsEnded(Guid gameId)
    {
        Game? game = null;
        try
        {
            game = _repo.GetGame(gameId);
        }
        catch (Exception ex)
        {
            return false;
        }
        return !game.IsGameEnded();
    }
    private void ShufflePlayers(Game game)
    {
        var randomNumber = _randomGenerator.GetRandomNumber(1, 2); // Будет ли возвращать два числа ? или только одно 
        var prevXplayer = game.XPlayer;
        game.XPlayer = randomNumber.Equals(1) ? game.OPlayer : prevXplayer;
        game.OPlayer = randomNumber.Equals(1) ? prevXplayer : game.OPlayer;
    }
    private IActionResult ConnectedToGame(Game game,Guid userId)
    {
        if (!game.State.Equals(State.WaitingForThePlayers) && (!game.XPlayer.Equals(Guid.Empty) || !game.OPlayer.Equals(Guid.Empty))) return BadRequest("Can't connected to game, game already starte");// переработать
        if (game.XPlayer.Equals(userId) || game.OPlayer.Equals(userId)) return BadRequest("User in the game");
        User? user = null;
        try
        {
            user = _repo.GetUser(userId);
        }
        catch (Exception ex)
        {
            return BadRequest("User not found");
        }
        var firstPlayerGuid = game.XPlayer;
        game.OPlayer = userId;
        game.State = State.PlayersXTurn;
        user.GameId = game.UUID;
        ShufflePlayers(game);
        if (game.XPlayer.Equals(user.UUID)) user.Symbol = UserSymbols.PlayerSymbol;
        else user.Symbol = UserSymbols.AISymbol;
        User firstPlayer = _repo.GetUser(firstPlayerGuid);
        if (game.XPlayer.Equals(firstPlayerGuid)) firstPlayer.Symbol = UserSymbols.PlayerSymbol;
        else firstPlayer.Symbol = UserSymbols.AISymbol;
        try
        {
            _repo.SetUser(user);
            _repo.SetUser(firstPlayer);
            _repo.SetGame(game);
        }
        catch (Exception ex)
        {
            return BadRequest("User not found");
        }
        return Ok(new UserDto(user));
    }
    private IActionResult PostGameWithUser(Game game,UserDto userDto)
    {
        if ((game.State.Equals(State.PlayersXTurn) && !game.XPlayer.Equals(userDto.UUID)) ||
            (game.State.Equals(State.PlayerOTurn) && !game.OPlayer.Equals(userDto.UUID))) 
            return BadRequest("Is not you'r turn");
        User user = userDto.ConvertToDomainUser();
        user.Field = userDto.UserField.ConvertToDomainField();
        if (_service.ValidationField(game,user)) return BadRequest("Your Field is not valid, please send valid field");
        game.State = game.State.Equals(State.PlayersXTurn) ? State.PlayerOTurn : State.PlayersXTurn;
        game.Field = user.Field;
        try
        {
            _repo.SetGame(game);
        }
        catch (Exception ex)
        {
            return BadRequest("Error while saving game");
        }
        var checkGameEnded = CheckGameEnded(game, userDto);
        if (checkGameEnded is not null) return checkGameEnded;
        return Ok("Your turn accepted wait opponent's turn");
    }
    private async Task<IActionResult> PostGameWithAi(Game game,UserDto user)
    {
        game.Field = user.UserField.ConvertToDomainField();
        IActionResult? checkGameEnded = CheckGameEnded(game,user);
        if (checkGameEnded is not null) return checkGameEnded;
        try
        {
            _repo.SetGame(game);
        }catch(Exception ex)
        {
            return BadRequest("Error while saving game");
        }
        try
        {
            await _service.NextTurn(game.UUID);
        }
        catch (Exception ex)
        {
            return BadRequest("Error while ai move next Turn game");
        }
        try
        {
            game = _repo.GetGame(game.UUID);
        }
        catch (Exception ex)
        {
            return BadRequest("Error then try get game again");
        }
        user.UserField.ConvertFromDomainField(game.Field);
        checkGameEnded = CheckGameEnded(game, user);
        if (checkGameEnded is not null){
            return checkGameEnded;
            }
        return Ok(user);
    }
    private IActionResult? CheckGameEnded(Game game, UserDto user)
    {
        var winner = user.UserField.ConvertToDomainField().CheckWin();
        if (!winner.Equals(-1))
        {
            switch (winner)
            {
                case 0:
                    game.State = State.Draw;
                    break;
                case 1:
                    game.State = State.PlayersXVictory;
                    break;
                case 2:
                    game.State = State.PlayersOVictory;
                    break;
                default:
                    break;
            }
            game.SetGameEnded(true);
            
            try
            {
                _repo.SetGame(game);
                if (!game.State.Equals(State.Draw))
                {
                    var xUserKpd = _repo.GetKpd(game.XPlayer);
                    xUserKpd.AddKpdToPercent(game);
                    Kpd? oUserKpd = null;
                    if (!game.OPlayer.Equals(Guid.Empty))
                    {
                        oUserKpd = _repo.GetKpd(game.OPlayer);
                        oUserKpd.AddKpdToPercent(game);
                    }
                    _repo.SetKpd(xUserKpd);
                    if (oUserKpd is not null)
                    {
                        _repo.SetKpd(oUserKpd);
                    }
                }
                
            }
            catch (Exception ex)
            {
                return BadRequest("Error while saving game");
            }
            return EndGame(game);
        }
        return null;
    }
    #endregion Methods.Private
}