namespace Tgs.GameRecord.JiliService.Core;

public record RedisOptions
{
    public string ConnectionString { get; set; } = "localhost";
}
