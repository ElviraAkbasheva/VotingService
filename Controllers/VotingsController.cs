using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VotingService.Data;
using VotingService.Models;
using VotingService.Services;

namespace VotingService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VotingsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IApartmentServiceClient _apartmentServiceClient;

    public VotingsController(ApplicationDbContext context, IApartmentServiceClient apartmentServiceClient)
    {
        _context = context;
        _apartmentServiceClient = apartmentServiceClient;
    }

    [HttpGet]
    public async Task<ActionResult<List<Voting>>> GetVotings()
    {
        var votings = await _context.Votings
            .Include(v => v.OwnersList)
            .ToListAsync();
        return Ok(votings);
    }

    [HttpPost]
    public async Task<ActionResult<Voting>> CreateVoting(CreateVotingRequestDto request)
    {
        if (request == null)
            return BadRequest();

        // Создаём пустое голосование
        var voting = new Voting
        {
            Id = Guid.NewGuid(),
            QuestionPut = request.QuestionPut,
            ResponseOptions = request.ResponseOptions,
            StartTime = DateTime.UtcNow,
            // TODO: заменить на передаваемую длительность
            EndTime = DateTime.UtcNow.AddDays(7),
            IsCompleted = false
        };

        // Для каждого дома получаем квартиры и собственников
        foreach (var houseId in request.HouseIds)
        {
            var apartments = await _apartmentServiceClient.GetApartmentsByHouseIdAsync(houseId);

            foreach (var apartment in apartments)
            {
                foreach (var user in apartment.Users)
                {
                    var owner = new Owner
                    {
                        Id = Guid.NewGuid(), // уникальный ID для записи
                        UserId = user.UserId,
                        ApartmentId = apartment.Id,
                        HouseId = houseId,
                        ApartmentArea = apartment.TotalArea,
                        Share = user.Share,
                        Response = "" // пока не голосовал
                    };
                    voting.OwnersList.Add(owner);
                }
            }
        }

        _context.Votings.Add(voting);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetVotings), new { id = voting.Id }, voting);
    }
}