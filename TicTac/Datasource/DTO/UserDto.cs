using System.Runtime.InteropServices;
using System.Security;
using Domain.Model;

namespace Datasource.DTO;
public class UserDto
{
    #region Properties
    public Int32 Id {get;set;}
    public Guid UUID { get; set; }
    public Guid GameId { get; set; }
    public bool IsLoginIn { get; set; } = false;
    public DateTime LoginTime { get; set; }
    public int Symbol { get; set; }
    #endregion Properties
    #region Constructors
    public UserDto()
    {
        UUID=Guid.Empty;
        GameId = Guid.Empty;
    }
    public UserDto(User user)
    {
        UUID = user.UUID;
        GameId = user.GameId;
        IsLoginIn = user.IsLogin;
        LoginTime = user.LoginTime;
        Symbol = user.Symbol;
    }

    #endregion Constructors
    #region Methods
    public User ConvertToUser() => new User(UUID,GameId,Symbol);
    #endregion Methods 
}
