namespace WebApi.DTO;

public class SignUpRequestDto
{
    #region Properties
    public string Login { get; set; } = null!;
    public string Password { get; set; } = null!;
    #endregion Properties
    #region Constructors
    public SignUpRequestDto(string login, string password)
    {
        SetLogin(login);
        SetPassword(password);
    }
    #endregion Constructors
    #region Methods
    public void SetLogin(string login)
    {
        if (CheckCorrectString(login)) throw new ArgumentException("login can't be null or empty");
        Login = login;
    }
    public void SetPassword(string password)
    {
        if (CheckCorrectString(password) || password.Length < 8) throw new ArgumentException("password can't be null or empty, or length less than 8 symbols");
        Password = password;
    }
    private bool CheckCorrectString(string str) => string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str) || str.Length < 5;
    #endregion Methods 
}
