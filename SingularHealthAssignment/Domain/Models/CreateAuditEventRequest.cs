namespace SingularHealthAssignment.Domain.Model
{
    public class CreateAuditEventRequest
    {
        public string ServiceName { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
    }
}
