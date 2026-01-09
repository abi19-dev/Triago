using EmailTriageAgent.Application.Services;
using EmailTriageAgent.Domain;
using EmailTriageAgent.Web.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace EmailTriageAgent.Web.Controllers;

[ApiController]
[Route("api/emails")]
public sealed class EmailsController : ControllerBase
{
    private readonly QueueService _queueService;
    private readonly EmailQueryService _emailQueryService;

    public EmailsController(QueueService queueService, EmailQueryService emailQueryService)
    {
        _queueService = queueService;
        _emailQueryService = emailQueryService;
    }

    [HttpPost]
    public async Task<ActionResult<EmailDto>> EnqueueAsync([FromBody] EmailCreateDto dto, CancellationToken ct)
    {
        var message = new EmailMessage
        {
            Id = Guid.NewGuid(),
            Sender = dto.Sender,
            Subject = dto.Subject,
            Body = dto.Body
        };

        await _queueService.EnqueueAsync(message, ct);
        return Ok(MapToDto(message));
    }

    [HttpGet]
    public async Task<ActionResult<List<EmailDto>>> GetRecentAsync([FromQuery] int take = 20, CancellationToken ct = default)
    {
        var messages = await _emailQueryService.GetRecentAsync(take, ct);
        return Ok(messages.Select(MapToDto).ToList());
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken ct)
    {
        var deleted = await _emailQueryService.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }

    private static EmailDto MapToDto(EmailMessage message)
    {
        return new EmailDto
        {
            Id = message.Id,
            Sender = message.Sender,
            Subject = message.Subject,
            Body = message.Body,
            ReceivedAt = message.ReceivedAt,
            Status = message.Status,
            Decision = message.Decision,
            SpamScore = message.Prediction?.SpamScore
        };
    }
}
