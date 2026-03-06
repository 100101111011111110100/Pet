using System.Security;

namespace Domain.Model;
public class User
{
    #region Properties
    public Guid UUID { get; set; } = Guid.NewGuid(); // уникальный пользовательский айди
    public Guid GameId { get; set; } = Guid.Empty; // привязка пользователя к игре
    public Int32 Symbol { get; set; } = 0;
    public DateTime LoginTime { get; set; }
    public bool IsLogin { get; set; } = false;
    public Field Field { get; set; } = new();
    #endregion Properties
    #region Constructors
    public User() { }
    public User(Guid id)=>UUID = id;
    public User(Guid userId, Guid gameId)
    {
        UUID = userId;
        GameId = gameId;
    }
    public User(Guid userId, Guid gameId, Int32 symbol)
    {
        UUID = userId;
        GameId = gameId;
        Symbol = symbol;
    }
    #endregion Constructors
    #region Methods
    public void SetGame(Guid gameId,int gameSymbol)
    {
        if ((!gameSymbol.Equals(UserSymbols.PlayerSymbol) &&!gameSymbol.Equals(UserSymbols.AISymbol)) || gameId.Equals(Guid.Empty)) throw new ArgumentException();
        GameId = gameId;
        Symbol = gameSymbol;
    }
    #endregion Methods
}