using SingularHealthAssignment.Domain.Model;

namespace SingularHealthAssignment.Application.Interfaces
{
    public interface IAuditEventRepository
    {
        Task AddAsync(AuditEvent auditEvent);
        Task<IEnumerable<AuditEvent>> GetAllAsync(string? serviceName, string? eventType, DateTime? from, DateTime? to);
        Task<IEnumerable<AuditEvent>> GetByIdsAsync(IEnumerable<Guid> ids);
    }
}
