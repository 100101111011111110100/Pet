using Datasource.DTO;
using Domain.Model;
using System.Security;

namespace Datasource.Iinterfaces;

public interface IGameRepository
{
    void DeleteGame(Guid gameId);
    Game GetGame(Guid gameId);
    Game GetEmptyUserToUserGame();
    List<Game> GetEmptyUserToUserGamesList();
    List<Game> GetFinishedGamesByUser(Guid userId);
    void SetGame(Game game);
    bool IsContainsGame(Guid gameId);
    User GetUser(Guid userId);
    User GetUser(string login);
    void SetUser(User user);
    Field GetField(Guid gameId);
    //bool ChangeUserCredentials(User user);
    UserCredential GetUserCredential(Guid userId);
    UserCredential GetUserCredential(string login);
    void SetUserCredential(UserCredential userCredential);
    void SetUserLoginIn(UserCredential userCredential);
    RefreshToken GetRefreshToken(string refreshToken);
    RefreshToken GetRefreshToken(Guid userId);
    void SetRefreshToken(RefreshToken refreshToken);
    Kpd GetKpd(Guid userId);
    void SetKpd(Kpd kpd);
    List<Kpd> GetNPlayersWithHighKpd(int n);

    void MigratePostgresDb();
}
