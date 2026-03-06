using Datasource.Iinterfaces;
using Datasource.Storage;
using Datasource.Services;
using Datasource.Security;

using Domain.Interfaces;
namespace Di;
public static class Configuration
{
    #region Properties
    public static IGameRepository GameRepository { get; }
    public static ITicTacService GameServices { get; }
    public static IPasswordHasher PasswordHasher { get; }
    public static IAuthService AuthService { get; }
    public static IRandomGenerator RandomGenerator { get; }
    public static IJwtProvider JwtProvider { get; }
    #endregion Properties
    #region Constructors
    static Configuration()
    {
        var jwtOptions = new Domain.Model.JwtOptions()
        {
            SecretKey = "super_secret_key_123456789101112",
            ExpiredMinutes = 60,
            Issuer = "TicTacApi",
            Audience = "TicTacClient"
        };

        RandomGenerator = new RandomGenerator();
        GameRepository = new GameRepositroy();
        PasswordHasher = new Pbkdf2PasswordHasher();
        GameServices = new TicTacService(GameRepository);
        JwtProvider = new JwtProvider(jwtOptions, GameRepository);
        AuthService = new AuthService(GameRepository, JwtProvider);
        
    }
    #endregion Constructors
    #region Methods
    #endregion Methods
}

