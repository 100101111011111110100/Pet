namespace Domain.Model;

public class Game
{
    #region Properties
        public Guid UUID { get; } = Guid.NewGuid();
        public Field Field { get; set; }
        public Opponents Opponents { get; set; }
        private bool _ended { get; set; } = false;
        public State State { get; set; } = State.WaitingForThePlayers;
        public Guid XPlayer { get; set; } = Guid.Empty;
        public Guid OPlayer { get; set; } = Guid.Empty;
        public DateTime Time { get; set; } = DateTime.UtcNow;
    //public Statio
    #endregion Properties
    #region Constructors
    public Game()
    {
        Field = new Field();
    }
    public Game(Field field)
    {
        Field = field;
    }
    public Game(Opponents opponents)
    {
        Field = new Field();
        Opponents = opponents;
    }
    public Game(Field field,Opponents opponents)
    {
        Field = field;
        Opponents = opponents;
    }
    public Game(Field field,Opponents opponents,Guid id)
    :this(field,opponents)
    {
        UUID=id;
    }
    public Game(Guid id,Field field,Opponents opponents,bool ended,State state,Guid xPlayer,Guid oPlayer)
        :this(field, opponents,id)
    {
        _ended= ended;
        State= state;
        XPlayer= xPlayer;
        OPlayer= oPlayer;
    }
    #endregion Constructors
    #region Methods
    public bool IsGameEnded() => _ended;
    public void SetGameEnded(bool isEnded) => _ended = isEnded;
    
    #endregion Methods
}
