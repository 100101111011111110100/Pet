using Domain.Model;

namespace Domain.Interfaces;

public interface ITicTacService
{
    //Task NextTurn(Field field);
    Task NextTurn(Guid gameUUID);

    //bool ValidationField(Guid gameId, Guid userId);
    //bool ValidationField(Field gameField, Field userField);
    bool ValidationField(Game game,User user);

    bool IsGameEnded(Field field);
    (bool, Guid) IsGameEnded(Game game);
    Player WhoIsWinner(Field field);
}
