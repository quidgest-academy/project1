using CSGenio.core.scheduler;
using CSGenio.framework;
using CSGenio.persistence;
using DbAdmin;

namespace Administration;

/// <summary>
/// Processes the asyncronous process queue tables for this system
/// </summary>
public class AsyncProcessScheduledTask : IScheduledTask
{

    /// <inheritdoc/>
    public List<ScheduledTaskOption> GetOptions() {
        return [
            new ScheduledTaskOption {
                PropertyName = "yearapp",
                DisplayName = "Year",
                Optional = true,
                Description = "Database year."
        },
        ];
    }

    /// <inheritdoc/>
    public Task Process(Dictionary<string, string> options, CancellationToken stoppingToken)
    {
        var year = ScheduledTaskExtensions.GetStringOption(options, "yearapp", Configuration.DefaultYear);

        PersistentSupport sp = null;
        try
        {
            var user = SysConfiguration.CreateWebAdminUser(year);
            sp = PersistentSupport.getPersistentSupport(user.Year, user.Name);
            sp.openTransaction();
            CSGenio.business.async.GenioWorker worker = new CSGenio.business.async.GenioWorker(sp, user);
            worker.Work();
            sp.closeTransaction();
        }
        catch (Exception ex)
        {
            sp?.rollbackTransaction();
            Log.Error($"Error handling WebApi call: {ex.Message}");
        }
        return Task.CompletedTask;
    }
}