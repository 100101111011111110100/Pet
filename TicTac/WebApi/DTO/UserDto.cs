using System.Security;
using Domain.Model;

namespace WebApi.DTO;

public class UserDto
{
    public Guid GameId { get; set; }
    public Guid UUID { get; set; }
    public FieldDto? UserField { get; set; } = null;
    public char Symbol { get; set; }
    #region Constructors
    public UserDto() { }
    public UserDto(User user)
    {
        UUID = user.UUID;
        GameId = user.GameId;
        UserField = new FieldDto();
        Symbol = GetUserSymbol(user.Symbol);
        UserField = new FieldDto(user.Field);
    }
    #endregion Constructors
    public void ConvertFromDomainUser(User user)
    {
        UUID = user.UUID;
        GameId = user.GameId;
        UserField = new FieldDto();
        Symbol = GetUserSymbol(user.Symbol);
    }

    public User ConvertToDomainUser() => new User(UUID,GameId);
    public void AddField(Field field)
    {
        UserField = new FieldDto();
        UserField.ConvertFromDomainField(field);
    }
    private char GetUserSymbol(int userSymbol) => userSymbol switch
    {
        UserSymbols.PlayerSymbol=>UserSymbols.PlayerCharSymbol,
        UserSymbols.AISymbol =>UserSymbols.AICharSymbol,
        _=>' '
    };
}