using Domain.Model;

namespace Domain.Interfaces;

public interface IJwtProvider
{
    String GenerateAccessToken(User user);
    Guid GetUUIDFromAccessToken(String accessToken);
    String GenerateRefreshToken(User user);
    Boolean ValidationAccessToken(String accessToken);
    Boolean ValidationRefreshToken(String firstToken);
    
}
