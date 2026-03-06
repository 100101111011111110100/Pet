using Datasource.DTO;
using Datasource.Iinterfaces;
using Domain.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security;




namespace Datasource.Storage;

public class PgStorage:DbContext,IGameBase
{
   #region Properties
   public DbSet<GameDto> Games { get; set; }
   public DbSet<UserDto> Users { get; set; }
   public DbSet<FieldDto> Fields { get; set; }
   public DbSet<UserCredentialsDto> Creds { get; set; }
   public DbSet<RefreshTokenDto> RefreshToken { get; set; }
   public DbSet<KpdDto> Kpd { get; set; }
    #endregion Properties
    #region Constructors
    internal PgStorage()
    {

    }
   public PgStorage(DbContextOptions<PgStorage> options)
        : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      //  modelBuilder.Entity<GameDto>()
      //  .HasAlternateKey(g => g.GameUId); // уникальный ключ

      //  modelBuilder.Entity<GameDto>()
      //.HasOne(g => g.FieldDto)
      //.WithMany()              // или .WithOne() если связь 1:1
      //.HasForeignKey(g => g.FieldId);
      //      //.HasPrincipalKey<GameDto>(g => g.GameUId);
      //  modelBuilder.Entity<GameDto>().ToTable("Games");
      //  modelBuilder.Entity<FieldDto>().ToTable("Fields");
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=test;Username=postgres;");//Password=postgres
        }
    }
    #endregion Constructors
    #region Methods
    #region IGameStorage
    public void MigrateDb() => this.Database.Migrate();
    public void DeletGame(Guid gameId){
        Games.Remove(GetGame(gameId));
        SaveChanges();
    }
    public GameDto GetGame(Guid gameId)=>Games.Include(g => g.FieldDto).First(game=>game.GameUId.Equals(gameId));
    public GameDto GetEmptyUserToUserGame() => Games.Include(g => g.FieldDto).First(game => game.Opponents.Equals(Opponents.UserVsUser) && game.OPlayer.Equals(Guid.Empty));
    public List<GameDto> GetEmptyUserToUserGamesList() => Games.Include(g => g.FieldDto).Where(game => game.Opponents.Equals(Opponents.UserVsUser) && game.OPlayer.Equals(Guid.Empty)).ToList();
    public void SetGame(GameDto game)
    {
        if (Games.Any(g=>g.GameUId.Equals(game.GameUId)))
        {
            var g = Games.Include(dbGame=> dbGame.FieldDto).First(dbGame => dbGame.GameUId.Equals(game.GameUId));
            SetField(game.FieldDto);
            g.IsEnded = game.IsEnded;
            g.Opponents = game.Opponents;
            g.XPlayer = game.XPlayer;
            g.OPlayer = game.OPlayer;
            g.State = game.State;
        }
        else
        {
            Games.Add(game);
        }
        SaveChanges();
    }
    #region User
    public UserDto GetUser(Guid userId)=>Users.First(user=>user.UUID.Equals(userId));
    public UserDto GetUser(string login)=> Users.First(user => user.UUID.Equals(Creds.First(c => c.Login.Equals(login)).UserId));

    public void SetUser(UserDto user)
    {
        if (Users.Any(u => u.UUID.Equals(user.UUID)))
        {
            var userFromDb = Users.First(u => u.UUID.Equals(user.UUID));
            userFromDb.GameId=user.GameId;
            userFromDb.IsLoginIn = user.IsLoginIn;
            userFromDb.Symbol = user.Symbol;
            userFromDb.LoginTime = user.LoginTime;
        }
        else
        {
            Users.Add(user);
        }
        SaveChanges();
    }
    public UserCredentialsDto GetUserCredential(Guid userId)=>Creds.First(c=>c.UserId.Equals(userId));
    public UserCredentialsDto GetUserCredential(string login) => Creds.First(c => c.Login.Equals(login));
    public void SetUserCredential(UserCredentialsDto userCredential)
    {
        if (Creds.Any(c => c.UserId.Equals(userCredential.UserId)))
        {
            var dbCred = Creds.First(c => c.UserId.Equals(userCredential.UserId));
            if (string.IsNullOrEmpty(userCredential.Login)|| string.IsNullOrWhiteSpace(userCredential.Login) || userCredential.Hash.Length.Equals(0) || userCredential.Salt.Length.Equals(0) || userCredential.Iteration < 0) throw new ArgumentException();
            dbCred.Login = userCredential.Login;
            dbCred.Hash = userCredential.Hash;
            dbCred.Salt = userCredential.Salt;
            dbCred.Iteration = userCredential.Iteration;
            dbCred.IsLoginIn = userCredential.IsLoginIn;
        }
        else
        {
            Creds.Add(userCredential);
        }
        SaveChanges();
    }
    public void SetUserLoginIn(UserCredentialsDto userCredential)
    {
        if (!Creds.Any(c => c.UserId.Equals(userCredential.UserId))) throw new Exception("User is not registred");
        var dbCreds = Creds.First(c => c.UserId.Equals(userCredential.UserId));
        dbCreds.IsLoginIn = userCredential.IsLoginIn;
        SaveChanges();
    }
    public List<GameDto> GetFinishedGameByUser(Guid userId)=> Games.Include(g=>g.FieldDto).Where(g => g.IsEnded && (g.XPlayer.Equals(userId) || g.OPlayer.Equals(userId))).ToList();

    #endregion User

    public FieldDto GetField(Guid gameId)=>Fields.First(field=>field.GameUUID.Equals(gameId));

    public List<KpdDto> GetNPlayersWithHighKpd(int n)=> Kpd.OrderBy(k => k.WinPercent).Take(n).ToList();
    
    public RefreshTokenDto GetRefreshToken(string refreshToken)=>RefreshToken.First(r=>r.TokenHash.Equals(refreshToken));
    public RefreshTokenDto GetRefreshToken(Guid userId)=> RefreshToken.First(r => r.UserId.Equals(userId) && !r.IsRevoked);
    public void SetRefreshToken(RefreshTokenDto refreshToken)
    {
        if (RefreshToken.Any(r => r.TokenHash.Equals(refreshToken.TokenHash)))
        {
            var dbToken = RefreshToken.First(r => r.TokenHash.Equals(refreshToken.TokenHash));
            dbToken.IsRevoked = refreshToken.IsRevoked;
            dbToken.ReplacedByToken = refreshToken.ReplacedByToken;
        }
        else
        {
            RefreshToken.Add(refreshToken);
        }
        SaveChanges();
    }
    public KpdDto GetKpd(Guid userId)=>Kpd.First(k=>k.UserId.Equals(userId));
    public void SetKpd(KpdDto kpd)
    {
        if (Kpd.Any(k=>k.UserId.Equals(kpd.UserId)))
        {
            var dbKpd = Kpd.First(k => k.UserId.Equals(kpd.UserId));
            dbKpd.WinPercent = kpd.WinPercent;
            dbKpd.LosPercent = kpd.LosPercent;
            dbKpd.GamesCount = kpd.GamesCount;
            dbKpd.WinCount = kpd.WinCount;
            dbKpd.LosCount = kpd.LosCount;
        }
        else
        {
            Kpd.Add(kpd);
        }
        SaveChanges();
    }
    #endregion IGameStorage
    #region  Private
    private void SetField(FieldDto field)
    {
        if (Fields.Any(f=>f.GameUUID.Equals(field.GameUUID)))
        {
            var fDb = Fields.First(f => f.GameUUID.Equals(field.GameUUID));
            fDb.MatrixJson = field.MatrixJson;
        }
        else
        {
            Fields.Add(field);
        }
            SaveChanges();
    }
    /*
    private UInt64 GetNextLastFieldElemId()
     {
         try
         {
             return Fields.Select(f=>f.Id).Max()+1;
         }
         catch
         {
             return UInt64.MinValue;
         }
     }
     private UInt64 GetNextLastGameElemId()
     {
         try
         {
             return Games.Select(g=>g.Id).Max()+1;
         }
         catch
         {
             return UInt64.MinValue;
         }
     }
     private UInt64 GetNextLastUserElemId()
     {
         try
         {
             return Users.Select(u=>u.Id).Max()+1;
         }
         catch
         {
             return UInt64.MinValue;
         }
     }*/
    #endregion Private
    #endregion Methods 
}
