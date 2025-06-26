using Microsoft.AspNetCore.Mvc;
using SingularHealthAssignment.Application.Interfaces;
using SingularHealthAssignment.Domain.Model;

namespace SingularHealthAssignment.Controllers
{
    [ApiController]
    [Route("events")]
    public class AuditEventsController : ControllerBase
    {
        private readonly IAuditEventRepository _repository;
        private readonly ILogger<AuditEventsController> _logger;

        public AuditEventsController(IAuditEventRepository repository, ILogger<AuditEventsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateAuditEventRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ServiceName) ||
                string.IsNullOrWhiteSpace(request.EventType) ||
                string.IsNullOrWhiteSpace(request.Payload))
                return BadRequest("Missing required fields.");

            var model = new AuditEvent
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                ServiceName = request.ServiceName,
                EventType = request.EventType,
                Payload = request.Payload
            };

            await _repository.AddAsync(model);
            return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] string? serviceName,
            [FromQuery] string? eventType,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var events = await _repository.GetAllAsync(serviceName, eventType, from, to);
            var totalCount = events.Count();
            var items = events.Skip((page - 1) * pageSize).Take(pageSize);

            return Ok(new
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = items
            });
        }

        [HttpPost("replay")]
        public async Task<IActionResult> Replay([FromBody] List<Guid> ids)
        {
            var events = await _repository.GetByIdsAsync(ids);
            foreach (var ev in events)
            {
                _logger.LogInformation($"Replaying event: {System.Text.Json.JsonSerializer.Serialize(ev)}");
            }

            return Ok(new { Replayed = events.Count() });
        }
    }
}
