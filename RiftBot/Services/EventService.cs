using System.Linq.Expressions;

namespace RiftBot;

public class EventService
{
    private readonly RiftBotContext _context;

    public EventService(RiftBotContext context)
    {
        _context = context;
    }

    public async Task<List<EventLog>> GetEventLogs(Expression<Func<EventLog, bool>> predicate) =>
        await _context.EventLog.AsNoTracking().Where(predicate).ToListAsync();
}