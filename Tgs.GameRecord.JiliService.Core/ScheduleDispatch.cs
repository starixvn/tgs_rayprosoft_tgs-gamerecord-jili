using System.Text.Json;
using StackExchange.Redis;
using Tgs.GameRecord.JiliService.Core.DispatchServices;

namespace Tgs.GameRecord.JiliService.Core
{
    internal class ScheduleDispatch : IScheduleDispatch
    {
        private readonly IDatabase _database;
        private readonly TimeSpan _expiry;

        public ScheduleDispatch(
            IDatabase database,
            TimeSpan expiry)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _expiry = expiry;
        }

        public Task<bool> ExistAsync(string key)
            => _database.KeyExistsAsync(key: key);

        public Task<bool> LockingAsync(string key, ScheduleDispatchCommand command)
            => _database.StringSetAsync(key: key, value: JsonSerializer.Serialize(command), expiry: _expiry, when: When.NotExists);

        public Task<bool> UnlockAsync(string key)
            => _database.KeyDeleteAsync(key: key);
    }
}
