namespace Domain.Model;
public class JwtOptions
{
    public string SecretKey { get; set; } = null!;
    public int ExpiredMinutes { get; set; }
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
}
