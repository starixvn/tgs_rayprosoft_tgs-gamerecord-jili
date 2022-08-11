namespace Tgs.GameRecord.JiliService;

public record RedisOptions
{
    public string ConnectionString { get; set; } = "localhost";
}
