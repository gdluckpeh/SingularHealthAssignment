using System.ComponentModel.DataAnnotations;

namespace SingularHealthAssignment.Domain.Model
{
    public class AuditEvent
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }

        [Required]
        public string ServiceName { get; set; }

        [Required]
        public string EventType { get; set; }

        [Required]
        public string Payload { get; set; }
    }

}
