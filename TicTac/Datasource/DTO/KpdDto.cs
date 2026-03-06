using Domain.Model;

namespace Datasource.DTO;

public class KpdDto
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public double WinPercent { get; set; }
    public double LosPercent { get; set; }
    public int GamesCount { get; set; }
    public int WinCount { get; set; }
    public int LosCount { get; set; }
    internal KpdDto()
    {

    }
    public KpdDto(Kpd kpd)
    {
        UserId = kpd.UserId;
        WinPercent = kpd.WinPercent;
        LosPercent = kpd.LosPercent;
        GamesCount = kpd.GamesCount;
        WinCount = kpd.WinCount;
        LosCount = kpd.LosCount;
    }
    public Kpd ConvertToDomainModel() => new Kpd()
    {
        UserId = this.UserId,
        WinPercent = this.WinPercent,
        LosPercent = this.LosPercent,
        GamesCount = this.GamesCount,
        WinCount = this.WinCount,
        LosCount = this.LosCount
    };
}
