using SingularHealthAssignment.Application.Interfaces;
using SingularHealthAssignment.Domain.Model;
using System.Collections.Concurrent;

namespace SingularHealthAssignment.Infrastructure.Repositories
{
    public class InMemoryAuditEventRepository : IAuditEventRepository
    {
        private readonly ConcurrentDictionary<Guid, AuditEvent> _events = new();

        public Task AddAsync(AuditEvent auditEvent)
        {
            _events[auditEvent.Id] = auditEvent;
            return Task.CompletedTask;
        }

        public Task<IEnumerable<AuditEvent>> GetAllAsync(string? serviceName, string? eventType, DateTime? from, DateTime? to)
        {
            var results = _events.Values.AsEnumerable();

            if (!string.IsNullOrEmpty(serviceName))
                results = results.Where(e => e.ServiceName == serviceName);

            if (!string.IsNullOrEmpty(eventType))
                results = results.Where(e => e.EventType == eventType);

            if (from.HasValue)
                results = results.Where(e => e.Timestamp >= from.Value);

            if (to.HasValue)
                results = results.Where(e => e.Timestamp <= to.Value);

            return Task.FromResult(results);
        }

        public Task<IEnumerable<AuditEvent>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            var results = _events.Values.Where(e => ids.Contains(e.Id));
            return Task.FromResult(results);
        }
    }
}
