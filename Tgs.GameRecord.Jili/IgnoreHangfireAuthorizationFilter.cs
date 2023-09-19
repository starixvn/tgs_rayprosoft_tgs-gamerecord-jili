using Hangfire.Dashboard;

namespace Tgs.GameRecord.Jili;

public class IgnoreHangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context) => true;
}
