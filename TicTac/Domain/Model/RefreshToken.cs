namespace Domain.Model;

public class RefreshToken
{
    public Guid UserId { get; set; }
    public String Token { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
    public DateTime CreatedAt { get; set; }
    public Boolean IsRevoked { get; set; } = false;
    public String ReplacedByToken { get; set; } = string.Empty;
}
