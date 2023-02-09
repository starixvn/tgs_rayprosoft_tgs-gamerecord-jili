namespace Tgs.GameRecord.JiliManualService;

public record NatsSettingOptions
{
    public string Group { get; set; } = default!;
    public string ManualSubject { get; set; } = default!;

    public string ManualScheduleSubject { get; set; } = default!;
}
