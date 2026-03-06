using Datasource.Iinterfaces;
using Domain.Interfaces;
using Domain.Model;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Datasource.Security;

public class JwtProvider: IJwtProvider
{
    #region Properties
    private IGameRepository _db;
    private readonly JwtOptions _jwtOptions;
    #endregion Properties
    #region Constructors
    public JwtProvider(JwtOptions jwtOptions, IGameRepository db)
    {
        _jwtOptions = jwtOptions;
        _db = db;
    }
    #endregion Constructors
    #region Methods

    public String GenerateAccessToken(User user) {
        var now = DateTime.UtcNow;
        var claims = GetClaims(user, DateTime.Now);
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)
        );

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(10),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    public Guid GetUUIDFromAccessToken(String accessToken)
    {
        var token = ParseAccessToken(accessToken);
        return Guid.Parse(token.FindFirst("uuid")?.Value);//token.FindFirst("uuid")?.Value;
    }
    public Boolean ValidationAccessToken(String accessToken)
    {
        try
        {
            ParseAccessToken(accessToken);
            return true;
        }
        catch (Exception ex)
        {
        }
        return false;
    }
    public String GenerateRefreshToken(User user) {
        var bytes = new byte[64]; // 512 бит
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
    
    
    public Boolean ValidationRefreshToken(String refreshToken) {
        RefreshToken? dbToken = null;
        try
        {
            dbToken = _db.GetRefreshToken(refreshToken);
        }catch(Exception ex)
        {
            return false;
        }
        if(dbToken.IsRevoked 
            || dbToken.Expiration < DateTime.UtcNow
            || !dbToken.ReplacedByToken.Equals(string.Empty))
        {
            return false;
        }

        return true;
    }
    private List<Claim> GetClaims(User user, DateTime time) => new List<Claim>() {
        new Claim(JwtRegisteredClaimNames.Sub, user.UUID.ToString()),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Iat,
            new DateTimeOffset(time).ToUnixTimeSeconds().ToString(),
            ClaimValueTypes.Integer64),
        new Claim("uuid", user.UUID.ToString())//,
        //new Claim(ClaimTypes.Role, user.)
    };
    private ClaimsPrincipal ParseAccessToken(string AccessToken)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)
        );
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = true,
            ValidIssuer = _jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = _jwtOptions.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(10)
        };
        return tokenHandler.ValidateToken(AccessToken, validationParameters, out SecurityToken validatedToken);
    }
    #endregion Methods 
}
