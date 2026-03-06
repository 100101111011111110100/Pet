using System.Text;

namespace Datasource.Security;

public static class BaseDecoder
{
    public static (string login, string password) DecodeBase(string baseValue)
    {
        var bytes = Convert.FromBase64String(baseValue);
        var decoded = Encoding.UTF8.GetString(bytes);
       
        var parts = decoded.Split(':', 2);
        if (parts.Length != 2)
            throw new UnauthorizedAccessException("Invalid auth header");

        return (parts[0], parts[1]);
    }
}
