using Domain.Interfaces;
using Domain.Model;
using Microsoft.VisualBasic;
using System.Security.Cryptography;

namespace Datasource.Security;
public class Pbkdf2PasswordHasher : IPasswordHasher
{
    #region Properties
    #endregion Properties
    #region Constructors
    #endregion Constructors
    #region Methods
    public HashedPassword Hash(string password,int iteration) {
        if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(password) || iteration <= 0) throw new ArgumentException();
        byte[] salt = RandomNumberGenerator.GetBytes(16); // 128-bit salt
        var pbkdf2 = new Rfc2898DeriveBytes(
           password,
           salt,
           iteration,
           HashAlgorithmName.SHA256);
        return new HashedPassword(pbkdf2.GetBytes(32),salt,iteration);
    }
    public bool Verify(string password, HashedPassword hashedPassword)
    {
        var newHashed = RepeatPassword(password, hashedPassword.Iterations,hashedPassword.Salt);
        if (!newHashed.Hash.Length.Equals(hashedPassword.Hash.Length)) return false;
        for(int i=0;i< newHashed.Hash.Length; i++)
        {
            if (!newHashed.Hash[i].Equals(hashedPassword.Hash[i])) return false;
        }
        return true;
    }
    private HashedPassword RepeatPassword(string password,int iteration, byte[] salt)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(password) || iteration <= 0 || salt.Length.Equals(0)) throw new ArgumentException();
        var pbkdf2 = new Rfc2898DeriveBytes(
           password,
           salt,
           iteration,
           HashAlgorithmName.SHA256);
        return new HashedPassword(pbkdf2.GetBytes(32), salt, iteration);
    }
    #endregion Methods 
}
