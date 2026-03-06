using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Datasource.Security;

public static class HashTokens
{
    public static string HashToken(string token)=> Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    public static bool ValidToken(string firstToken, string hashedToken) => hashedToken.SequenceEqual(firstToken);
}
