using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VotingService.Data;
using VotingService.Models;

namespace VotingService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VotingsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public VotingsController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Получить все голосования (с владельцами)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<Voting>>> GetVotings()
    {
        var votings = await _context.Votings
            .Include(v => v.OwnersList)
            .ToListAsync();
        return Ok(votings);
    }

    /// <summary>
    /// Создать новое голосование
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Voting>> CreateVoting(Voting voting)
    {
        if (voting == null)
            return BadRequest();

        voting.Id = Guid.NewGuid();
        voting.StartTime = DateTime.UtcNow;
        // TODO: заменить на передаваемую длительность
        voting.EndTime = DateTime.UtcNow.AddDays(7);
        _context.Votings.Add(voting);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetVotings), new { id = voting.Id }, voting);
    }
}