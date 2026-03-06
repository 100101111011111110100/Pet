using Domain.Model;
namespace Domain.Interfaces;

public interface IPasswordHasher
{
    #region Properties
    #endregion Properties
    #region Constructors
    #endregion Constructors
    #region Methods
    public HashedPassword Hash(string password,int iteration);
    public bool Verify(string password, HashedPassword hashedPassword);
    #endregion Methods 
}
