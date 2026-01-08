using EmailTriageAgent.Application.Services;
using EmailTriageAgent.Web.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace EmailTriageAgent.Web.Controllers;

[ApiController]
[Route("api/reviews")]
public sealed class ReviewsController : ControllerBase
{
    private readonly ReviewService _reviewService;

    public ReviewsController(ReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpPost]
    public async Task<IActionResult> SubmitAsync([FromBody] ReviewCreateDto dto, CancellationToken ct)
    {
        await _reviewService.SubmitReviewAsync(dto.EmailId, dto.Label, dto.Reviewer, ct);
        return Ok();
    }
}
