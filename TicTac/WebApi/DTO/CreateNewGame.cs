using Domain.Model;
namespace WebApi.DTO;

public class CreateNewGame
{
    public Opponents Opponents { get; set; }
    public Guid UserId { get; set; }
}