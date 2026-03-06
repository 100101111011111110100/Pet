using Datasource.DTO;
using Datasource.Iinterfaces;
using Datasource.Security;
using Domain.Interfaces;
using Domain.Model;

namespace Datasource.Services;

public class AuthService: IAuthService
{
    #region Properties
    private readonly IGameRepository _repo;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;
    #endregion Properties
    #region Constructors
    public AuthService(IGameRepository repo,IJwtProvider jwtProvider) {
        _repo = repo;
        _passwordHasher = new Pbkdf2PasswordHasher();
        _jwtProvider = jwtProvider;
    }
    #endregion Constructors
    #region Methods
    public bool SignUp(string Login, string Password)
    {
        UserCredential? creds = FindUserCredentialsInBd(Login);
        if (creds is not null) return false;
        var user = new User();
        creds = new UserCredential(user.UUID, Login, _passwordHasher.Hash(Password, 1));
        var userKpd = new Kpd() { UserId = creds.UserId };
        var refreshToken = new RefreshToken()
        {
            UserId = user.UUID,
            Token = HashTokens.HashToken(_jwtProvider.GenerateRefreshToken(user)),
            CreatedAt = DateTime.UtcNow,
            Expiration = DateTime.UtcNow.AddDays(7)
        };
        try
        {
            _repo.SetUser(user);
            _repo.SetUserCredential(creds);
            _repo.SetKpd(userKpd);
            _repo.SetRefreshToken(refreshToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.Message}\n{ex.InnerException}");
            return false;
        }
        return true;
    }

    //JwtBearer
    public JwtResponse SignIn(JwtRequest request)
    {
        var userCreds = _repo.GetUserCredential(request.Login);
        if (!_passwordHasher.Verify(request.Password, userCreds.Password)) throw new Exception();
        JwtResponse response = new();
        var user = _repo.GetUser(userCreds.UserId);
        response.accessToken = _jwtProvider.GenerateAccessToken(user);
        response.refreshToken = _jwtProvider.GenerateRefreshToken(user);
        var newRefreshToken = new RefreshToken()
        {
            UserId = user.UUID,
            Token = HashTokens.HashToken(response.refreshToken),
            CreatedAt = DateTime.UtcNow,
            Expiration = DateTime.UtcNow.AddDays(7)
        };
        try
        {
            var refresh = _repo.GetRefreshToken(userCreds.UserId);
            refresh.ReplacedByToken = newRefreshToken.Token;
            refresh.IsRevoked = true;
            _repo.SetRefreshToken(refresh);
        }
        catch(Exception ex) { 
            Console.WriteLine($"AuthService.SignIn\n{ex.Message} ||| {ex.InnerException} \n {ex.StackTrace}");
        }
        _repo.SetRefreshToken(newRefreshToken);
        return response;
    }
    public JwtResponse UpdateAccessToken(RefreshToken token)
    {
        JwtResponse response = new JwtResponse();
        //if (!_jwtProvider.ValidationRefreshToken(token.Token)) throw new Exception("not valid token");
        response.refreshToken = token.Token;
        response.accessToken = _jwtProvider.GenerateAccessToken(_repo.GetUser(token.UserId));
        return response;
    }
    public JwtResponse UpdateRefreshToken(RefreshToken token)
    {
        if (!_jwtProvider.ValidationRefreshToken(token.Token)) throw new Exception("not valid token");
        var user = _repo.GetUser(token.UserId);
        var refreshToken = _jwtProvider.GenerateRefreshToken(user);
        token.ReplacedByToken = HashTokens.HashToken(refreshToken);
        token.IsRevoked = true;
        _repo.SetRefreshToken(token);
        _repo.SetRefreshToken(new RefreshToken()
        {
            UserId = user.UUID,
            Token = HashTokens.HashToken(refreshToken),
            CreatedAt = DateTime.UtcNow,
            Expiration = DateTime.UtcNow.AddDays(7),
        });
        return new JwtResponse()
        {
            accessToken = _jwtProvider.GenerateAccessToken(user),
            refreshToken = refreshToken
     
        };
    }
    //public JwtResponse GenerateNewAccessToken(User user) => new JwtResponse()
    //{
    //    accessToken = _jwtProvider.GenerateAccessToken(user)
    //};
    /* Base auth
    public Guid? SignIn(string Base)
    {
        var inputCreds = BaseDecoder.DecodeBase(Base);
        UserCredential? creds = FindUserCredentialsInBd(inputCreds.login);
        if (creds is null) return null;
        else if (creds.IsLoggedIn) return null;
        if(_passwordHasher.Verify(inputCreds.password, creds.Password))
        {
            var user = _repo.GetUser(creds.UserId);
            user.LoginTime = DateTime.UtcNow;
            user.IsLogin = true;
            _repo.SetUser(user);
            return creds.UserId;
        }
        return null;
    }
    public Guid SignIn(string Login,string Password)
    {
        var user = _repo.GetUser(Login);
        var uCreds = _repo.GetUserCredential(user.UUID);
        if(_passwordHasher.Verify(Password, uCreds.Password))
        {
            uCreds.SetLogin(true);
            _repo.SetUserCredential(uCreds);
            return user.UUID;
        }
        return Guid.Empty;
    }*/
    #region Private
    private UserCredential? FindUserCredentialsInBd(string login)
    {
        try
        {
            return _repo.GetUserCredential(login);
        }
        catch
        {
            return null;
        }
        
    }
    #endregion Private

    #endregion Methods 
}
