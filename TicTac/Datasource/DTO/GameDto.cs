/*
    Не знаю что сюда писать , вопрос в том как портировать поле
*/

using Domain.Model;

namespace Datasource.DTO;
public class GameDto
{
    #region Properties
        public Int32 Id {get;set;} 
        public Int32 FieldId {get;set;}
        public Guid GameUId {get;set; } = Guid.Empty;
        public Guid XPlayer { get; set; } = Guid.Empty;
        public Guid OPlayer { get; set; } = Guid.Empty;
        public FieldDto FieldDto {get;set;}
        public Opponents Opponents { get; set; } = Opponents.UserVsAi;
        public State State { get; set; } = State.WaitingForThePlayers;
        public bool IsEnded { get; set; } = false;
        public DateTime Time { get; set; } 
    #endregion Properties
    #region Constructors
    public GameDto()
    {

    }
    public GameDto(Game game){
        GameUId = game.UUID;
        FieldDto = new FieldDto(game.UUID,game);
        Opponents = game.Opponents;
        FieldId=FieldDto.Id;
        IsEnded = game.IsGameEnded();
        State = game.State;
        XPlayer = game.XPlayer;
        OPlayer = game.OPlayer;
        Time = game.Time;
    }
    
    #endregion Constructors
    #region Methods
    public Game ConvertToGame()=> new Game(GameUId, FieldDto.ConvertToField(), Opponents, IsEnded,State,XPlayer,OPlayer);
    
    #endregion Methods 
}