using Tgs.GameRecord.Jili.Core;
namespace Tgs.GameRecord.Jili;

public record RedisOptions
{
    public string ConnectionString { get; set; } = "localhost";
}
