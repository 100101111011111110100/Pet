using Datasource.DTO;
namespace Datasource.Iinterfaces;

using Domain.Model;
using System.Security;

public interface IGameBase
{
    void DeletGame(Guid gameId);
    GameDto GetGame(Guid gameId);
    GameDto GetEmptyUserToUserGame();
    List<GameDto> GetEmptyUserToUserGamesList();
    List<GameDto> GetFinishedGameByUser(Guid userId);
    void SetGame(GameDto game);
    UserDto GetUser(Guid userId);
    UserDto GetUser(string login);
    void SetUser(UserDto user);
    UserCredentialsDto GetUserCredential(Guid userId);
    UserCredentialsDto GetUserCredential(string login);
    void SetUserCredential(UserCredentialsDto userCredential);
    void SetUserLoginIn(UserCredentialsDto userCredential);
    FieldDto GetField(Guid gameId);
    RefreshTokenDto GetRefreshToken(string refreshToken);
    RefreshTokenDto GetRefreshToken(Guid userId);
    void SetRefreshToken(RefreshTokenDto refreshToken);
    KpdDto GetKpd(Guid userId);
    void SetKpd(KpdDto kpd);
    List<KpdDto> GetNPlayersWithHighKpd(int n);
    void MigrateDb();


}