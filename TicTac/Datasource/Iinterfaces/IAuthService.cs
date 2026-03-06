using Datasource.DTO;
using Domain.Model;

namespace Datasource.Iinterfaces;

public interface IAuthService
{
    /// <summary>
    /// Регистрация
    /// </summary>
    /// <param name="Login"></param>
    /// <param name="Password"></param>
    /// <returns></returns>
    bool SignUp(string Login, string Password);
    /// <summary>
    /// Авторизация
    /// </summary>
    /// <param name="Login"></param>
    /// <param name="Password"></param>
    /// <returns></returns>
    //Base auth
    //Guid? SignIn(string Base);
    //Guid SignIn(string Login, string Password);

    //JwtBearer
    JwtResponse SignIn(JwtRequest request);
    JwtResponse UpdateAccessToken(RefreshToken token);
    JwtResponse UpdateRefreshToken(RefreshToken token);
}
