using BookReadingTracker.Domain.Features.ReadingProgress;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookReadingTracker.WebApi.Features.ReadingProgress;

[Route("api/reading-progress")]
[ApiController]
[Authorize]
public class ReadingProgressController : ControllerBase
{
    private readonly ReadingProgressService _readingProgressService;

    public ReadingProgressController(ReadingProgressService readingProgressService)
    {
        _readingProgressService = readingProgressService;
    }

    // GET /api/reading-progress             → all progress
    // GET /api/reading-progress?status=Completed  → reading history
    [HttpGet]
    public async Task<IActionResult> GetMyProgress([FromQuery] string? status)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _readingProgressService.GetMyProgressAsync(userId, status);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProgress(Guid id, UpdateProgressRequest request)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _readingProgressService.UpdateProgressAsync(id, request, userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
