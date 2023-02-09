using Tgs.GameRecord.JiliService.Core;
namespace Tgs.GameRecord.JiliService;

public record NatsSettingOptions
{
    public string Group { get; set; } = default!;
    public string Subject { get; set; } = default!;
}
