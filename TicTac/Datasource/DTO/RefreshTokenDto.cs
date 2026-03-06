using Datasource.Security;
using Domain.Model;
using System.Security.Cryptography;
using System.Text;

namespace Datasource.DTO;

public class RefreshTokenDto
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public String TokenHash { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
    public DateTime CreatedAt { get; set; }
    public Boolean IsRevoked { get; set; }
    public String ReplacedByToken { get; set; } = string.Empty;

    internal RefreshTokenDto() { }
    public RefreshTokenDto(RefreshToken token)
    {
        UserId = token.UserId;
        TokenHash = token.Token;
        Expiration = token.Expiration;
        CreatedAt = token.CreatedAt;
        IsRevoked = token.IsRevoked;
        ReplacedByToken = token.ReplacedByToken;
    }

    public RefreshToken ConvertToDomainModel() => new RefreshToken() {
        UserId = UserId,
        Token = TokenHash,
        Expiration = Expiration,
        CreatedAt = CreatedAt,
        IsRevoked = IsRevoked,
        ReplacedByToken = ReplacedByToken
    };
}
