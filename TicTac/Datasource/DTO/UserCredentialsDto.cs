using Domain.Model;

namespace Datasource.DTO;

public class UserCredentialsDto
{
    #region Properties
    public Int32 Id { get; set; }
    public Guid UserId { get; set; } = Guid.Empty;
    public string Login { get; set; } = string.Empty;
    public byte[] Hash { get; set; }
    public byte[] Salt { get; set; }
    public int Iteration { get; set; }
    public bool IsLoginIn { get; set; } = false;
    #endregion Properties
    #region Constructors
    public UserCredentialsDto(Guid userId,string login, byte[] hash, byte[] salt,int iteration)
    {
        UserId = userId;
        Login = login;
        Iteration = iteration;
        Salt = salt;
        Hash = hash;
    }
    public UserCredentialsDto(Guid userId, string login, byte[] hash, byte[] salt, int iteration, bool isLoginIn) :this(userId,login,hash,salt,iteration)
    {
        IsLoginIn = isLoginIn;
    }
    public UserCredentialsDto(string login, HashedPassword password)
        : this(password.UserId, login, password.Hash, password.Salt, password.Iterations) { }
    public UserCredentialsDto(UserCredential creds)
        :this(creds.UserId,creds.Login,creds.Password.Hash,creds.Password.Salt,creds.Password.Iterations,creds.IsLoggedIn) { }
    #endregion Constructors
    #region Methods
    public UserCredential ConvertToDomainUserCredential() => new UserCredential(UserId, Login, new HashedPassword(Hash, Salt, Iteration) { UserId=this.UserId});
    #endregion Methods 
}
