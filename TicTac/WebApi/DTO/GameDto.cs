using Domain.Model;

namespace WebApi.DTO;
public class GameDto
{
    public Guid GUUID { get; set; }
    public FieldDto GameField { get; set; }
    public Opponents Opponents {get;set;}
    public GameDto()
    {
        GUUID = Guid.Empty;
        GameField = new FieldDto();
        Opponents = Opponents.UserVsAi;
    }
    public GameDto(Game game)
    {
        GUUID = game.UUID;
        var fieldDto = new FieldDto();
        fieldDto.ConvertFromDomainField(game.Field);
        GameField = fieldDto;
        Opponents = game.Opponents;
    }
    public void ConvertFromDomainGame(Game game)
    {
        GUUID = game.UUID;
        var FieldDto = new FieldDto();
        FieldDto.ConvertFromDomainField(game.Field);
        GameField = FieldDto;
    }
    public Game ConvertToDomainGame() =>
        new Game(
            GameField.ConvertToDomainField(),
            Opponents
        );
}