using EmailTriageAgent.Application.Services;
using EmailTriageAgent.Domain;
using EmailTriageAgent.Web.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace EmailTriageAgent.Web.Controllers;

[ApiController]
[Route("api/settings")]
public sealed class SettingsController : ControllerBase
{
    private readonly SettingsService _settingsService;

    public SettingsController(SettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    [HttpGet]
    public async Task<ActionResult<SettingsDto>> GetAsync(CancellationToken ct)
    {
        var settings = await _settingsService.GetAsync(ct);
        return Ok(MapToDto(settings));
    }

    [HttpPut]
    public async Task<ActionResult<SettingsDto>> UpdateAsync([FromBody] SettingsDto dto, CancellationToken ct)
    {
        var updated = await _settingsService.UpdateAsync(new SystemSettings
        {
            ReviewThreshold = dto.ReviewThreshold,
            BlockThreshold = dto.BlockThreshold,
            RetrainEnabled = dto.RetrainEnabled,
            GoldThreshold = dto.GoldThreshold,
            SpamKeywordsCsv = dto.SpamKeywordsCsv
        }, ct);

        return Ok(MapToDto(updated));
    }

    private static SettingsDto MapToDto(SystemSettings settings)
    {
        return new SettingsDto
        {
            ReviewThreshold = settings.ReviewThreshold,
            BlockThreshold = settings.BlockThreshold,
            RetrainEnabled = settings.RetrainEnabled,
            GoldThreshold = settings.GoldThreshold,
            NewGoldSinceLastTrain = settings.NewGoldSinceLastTrain,
            SpamKeywordsCsv = settings.SpamKeywordsCsv
        };
    }
}
