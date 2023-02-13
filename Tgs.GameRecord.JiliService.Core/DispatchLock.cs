using System.Text.Json;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Tgs.GameRecord.JiliService.Core.DispatchServices;

namespace Tgs.GameRecord.JiliService.Core;

internal class DispatchLock : IDispatchLock
{
    private readonly IDatabase _database;
    private readonly TimeSpan _expiry;
    private readonly GameRecordInfoOptions _gameRecordInfo;

    public DispatchLock(
        IOptions<GameRecordInfoOptions> options,
        IDatabase database)
    {
        _gameRecordInfo = options.Value;
        _database = database ?? throw new ArgumentNullException(nameof(database));
        _expiry = TimeSpan.FromMinutes(_gameRecordInfo.LockMinute);
    }

    public Task<bool> LockingAsync(string key, ScheduleDispatchCommand command)
        => _database.StringSetAsync(key: key, value: JsonSerializer.Serialize(command), expiry: _expiry, when: When.NotExists);

    public Task<bool> LockingAsync(string key, ManualDispatchCommand command)
        => _database.StringSetAsync(key: key, value: JsonSerializer.Serialize(command), expiry: _expiry, when: When.NotExists);

    public Task<bool> UnlockAsync(string key)
        => _database.KeyDeleteAsync(key: key);
}
