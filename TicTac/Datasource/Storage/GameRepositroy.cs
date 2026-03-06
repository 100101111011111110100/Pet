using Datasource.DTO;
using Datasource.Iinterfaces;
using Domain.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security;

namespace Datasource.Storage;

public class GameRepositroy:IGameRepository
{
    #region Properties
    // private IGameStorage gameStorage { get; set; }
    private IGameBase gameStorage {get;set;}
    #endregion Properties
    #region Constructors
    // public GameRepositroy(IGameStorage storage) {
    //     gameStorage= storage;
    // }
    public GameRepositroy(){
        var pgStorage =  new PgStorage(new DbContextOptions<PgStorage>());
        pgStorage.Database.EnsureCreated();
        // pgStorage.Database.Migrate();
        gameStorage = pgStorage;
    }
    public GameRepositroy(IGameBase storage)=>gameStorage=storage;

    #endregion Constructors
    #region Methods
    public void MigratePostgresDb() => gameStorage.MigrateDb();
    public void DeleteGame(Guid gameId) => gameStorage.DeletGame(gameId);
    public Game GetGame(Guid gameId)=>gameStorage.GetGame(gameId).ConvertToGame();
    public Game GetEmptyUserToUserGame()=> gameStorage.GetEmptyUserToUserGame().ConvertToGame();
    public List<Game> GetEmptyUserToUserGamesList()
    {
        var list = gameStorage.GetEmptyUserToUserGamesList();
        return ConvertListDbGameDtoToDomainModel(list);
    }
    public List<Game> GetFinishedGamesByUser(Guid userId)
    {
        var list = gameStorage.GetFinishedGameByUser(userId);
        return ConvertListDbGameDtoToDomainModel(list);
    }
    public void SetGame(Game game)=>gameStorage.SetGame(new GameDto(game));
    public User GetUser(Guid userId)=>gameStorage.GetUser(userId).ConvertToUser();
    public User GetUser(string login)=>gameStorage.GetUser(login).ConvertToUser();
    public void SetUser(User user)=>gameStorage.SetUser(new UserDto(user));
    public Field GetField(Guid gameId)=> gameStorage.GetField(gameId).ConvertToField();

    public bool IsContainsGame(Guid gameId)
    {
        try
        {
            gameStorage.GetGame(gameId);
            return true;
        }
        catch
        {
            return false;
        }
    }
    public UserCredential GetUserCredential(Guid userId) => gameStorage.GetUserCredential(userId).ConvertToDomainUserCredential();
    public UserCredential GetUserCredential(string login) => gameStorage.GetUserCredential(login).ConvertToDomainUserCredential();
    public void SetUserCredential(UserCredential userCredential)=>gameStorage.SetUserCredential(new UserCredentialsDto(userCredential));
    public void SetUserLoginIn(UserCredential userCredential)=> gameStorage.SetUserCredential(new UserCredentialsDto(userCredential));
    public RefreshToken GetRefreshToken(string refreshToken)=>gameStorage.GetRefreshToken(refreshToken).ConvertToDomainModel();
    public RefreshToken GetRefreshToken(Guid userId)=> gameStorage.GetRefreshToken(userId).ConvertToDomainModel();
    public void SetRefreshToken(RefreshToken refreshToken) => gameStorage.SetRefreshToken(new RefreshTokenDto(refreshToken));
    public Kpd GetKpd(Guid userId) => gameStorage.GetKpd(userId).ConvertToDomainModel();
    public void SetKpd(Kpd kpd) => gameStorage.SetKpd(new KpdDto(kpd));
    public List<Kpd> GetNPlayersWithHighKpd(int n) {
        List<Kpd> list = [];
        gameStorage.GetNPlayersWithHighKpd(n).ForEach(k => list.Add(k.ConvertToDomainModel()));
        return list;
    }
    private List<Game> ConvertListDbGameDtoToDomainModel(List<GameDto> list)
    {
        List<Game> resList = new List<Game>();
        list.ForEach(l => resList.Add(l.ConvertToGame()));
        return resList;
    }

    #endregion Methods 
}
