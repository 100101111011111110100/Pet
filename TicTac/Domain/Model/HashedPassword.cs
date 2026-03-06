namespace Domain.Model;
public sealed class HashedPassword
{
    #region Properties
    public Guid UserId { get; set; }
    public byte[] Hash { get; }
    public byte[] Salt { get; }
    public int Iterations { get; }
    #endregion Properties
    #region Constructors
    public HashedPassword(byte[] hash,byte[] salt,int iterations)
    {
        UserId = Guid.Empty;
        Hash = hash;
        Salt = salt;
        Iterations = iterations;
    }
    public HashedPassword(Guid userId,byte[] hash, byte[] salt,int iterations)
        :this(hash,salt,iterations)
    {
        UserId = userId;
    }
    #endregion Constructors
    #region Methods
    #endregion Methods 
}
