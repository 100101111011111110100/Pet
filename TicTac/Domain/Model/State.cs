namespace Domain.Model;

public enum State
{
    WaitingForThePlayers=0,// - Ожидание игроков;
    PlayersTurn,// - Ход игрока с UUID;
    Draw,// - Ничья;
    PlayersXVictory,// - Победа игрока с UUID.
    PlayersOVictory,// - Победа игрока с UUID.
    PlayersXTurn,
    PlayerOTurn
}
