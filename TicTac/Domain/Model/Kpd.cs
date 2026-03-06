namespace Domain.Model;

public class Kpd
{
    public Guid UserId { get; set; }
    public double WinPercent { get; set; }
    public double LosPercent { get; set; }
    public int GamesCount { get; set; }
    public int WinCount { get; set; }
    public int LosCount { get; set; }

    public void SetKpd(List<Game> games)
    {
        if (games.Any(g => g.State.Equals(State.Draw))) throw new ArgumentException();
        GamesCount = games.Count;
        WinCount = games.Where(g => 
        (g.State.Equals(State.PlayersXVictory)&&g.XPlayer.Equals(UserId))
        || (g.State.Equals(State.PlayersOVictory) && g.OPlayer.Equals(UserId))
        ).Count();
        LosCount = games.Where(g =>
        (g.State.Equals(State.PlayersXVictory) && !g.XPlayer.Equals(UserId))
        || (g.State.Equals(State.PlayersOVictory) && !g.OPlayer.Equals(UserId))
        ).Count();
        CalculateNewKpd();
    }

    public void AddKpdToPercent(Game game)
    {
        if (game.State.Equals(State.Draw)) throw new ArgumentException(nameof(game));
        if ((game.State.Equals(State.PlayersXVictory) && game.XPlayer.Equals(UserId))
        || (game.State.Equals(State.PlayersOVictory) && game.OPlayer.Equals(UserId))) WinCount++;
        else if ((game.State.Equals(State.PlayersXVictory) && !game.XPlayer.Equals(UserId))
        || (game.State.Equals(State.PlayersOVictory) && !game.OPlayer.Equals(UserId))) LosCount++;
        GamesCount++;
        CalculateNewKpd();
    }
    private void CalculateNewKpd()
    {
        WinPercent = (double)WinCount / (double)GamesCount *100;
        LosPercent = (double)LosCount / (double)GamesCount *100;
    }
}
