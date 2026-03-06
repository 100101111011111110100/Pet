namespace Domain.Model;

public class UserCredential
{
    #region Properties
    public Guid UserId { get; set; }
    public string Login { get; set; }
    public HashedPassword Password { get; set; }
    private bool _isLogined { get; set; } = false;
    public bool IsLoggedIn { get=>_isLogined; }
    #endregion Properties
    #region Constructors
    public UserCredential(Guid userId, string login, HashedPassword password)
    {
        UserId = userId;
        Login = login;
        Password = password;
    }
    public UserCredential(Guid userId, string login, HashedPassword password, bool isLogined) : this(userId, login, password)
    {
        _isLogined = isLogined;
    }

    #endregion Constructors
    #region Methods
    public void SetLogin(bool isLoggedIn) => _isLogined = isLoggedIn;
    #endregion Methods 
}
