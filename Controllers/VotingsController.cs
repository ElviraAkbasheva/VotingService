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

    [HttpPost("{id}/vote")]
    public async Task<ActionResult> SubmitVote(Guid id, VoteRequestDto request)
    {
        // 1. Найти голосование с владельцами
        var voting = await _context.Votings
            .Include(v => v.OwnersList)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (voting == null)
            return NotFound("Голосование не найдено");

        // 2. Проверить, активно ли голосование
        if (voting.IsCompleted || voting.EndTime < DateTime.UtcNow)
        {
            return BadRequest("Голосование уже завершено");
        }

        // 3. Найти владельца с такими UserId и ApartmentId
        var owner = voting.OwnersList
            .FirstOrDefault(o => o.UserId == request.UserId && o.ApartmentId == request.ApartmentId);

        if (owner == null)
        {
            return BadRequest("Указанный пользователь не является владельцем в этой квартире");
        }

        // 4. Проверить, что он ещё не голосовал
        if (!string.IsNullOrEmpty(owner.Response))
        {
            return BadRequest("Пользователь уже проголосовал");
        }

        // 5. Вычислить TotalHouseArea для этого дома (в рамках этого голосования)
        var totalHouseArea = voting.OwnersList
            .Where(o => o.HouseId == owner.HouseId) // только из этого дома
            .Sum(o => o.ApartmentArea);

        if (totalHouseArea == 0)
        {
            return BadRequest("Общая площадь дома не может быть нулевой");
        }

        // 6. Рассчитать вес голоса
        owner.VoteWeight = (owner.ApartmentArea * owner.Share) / totalHouseArea;

        // 7. Принять голос
        owner.Response = request.Response;

        await _context.SaveChangesAsync();

        return Ok("Голос принят с весом: " + owner.VoteWeight);
    }

    [HttpGet("{id}/results")]
    public async Task<ActionResult<VotingResultDto>> GetVotingResults(Guid id)
    {
        var voting = await _context.Votings
            .Include(v => v.OwnersList)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (voting == null)
            return NotFound("Голосование не найдено");

        // Проверим, завершено ли голосование:
        // - Или флаг IsCompleted = true
        // - Или время вышло
        // - Или все собственники в этом голосовании уже проголосовали
        bool isCompleted = voting.IsCompleted
                        || voting.EndTime < DateTime.UtcNow
                        || voting.OwnersList.All(o => !string.IsNullOrEmpty(o.Response));

        if (!isCompleted)
        {
            return BadRequest("Голосование ещё активно, результаты недоступны");
        }

        // Найти всех, кто проголосовал
        var votedOwners = voting.OwnersList
            .Where(o => !string.IsNullOrEmpty(o.Response))
            .ToList();

        // Подсчитать общий вес проголосовавших
        var totalVotedWeight = votedOwners.Sum(o => o.VoteWeight);

        // Сгруппировать по вариантам ответа и суммировать вес. Выдать результат в %
        var responses = votedOwners
            .GroupBy(o => o.Response)
            .ToDictionary(
                g => g.Key,
                g => totalVotedWeight > 0
                    ? Math.Round((double)(g.Sum(o => o.VoteWeight) / totalVotedWeight) * 100, 2)
                    : 0.0
            );

        // Определить текст решения
        string decision = string.IsNullOrEmpty(voting.Decision)
            ? "Решение не опубликовано"
            : voting.Decision;

        var result = new VotingResultDto
        {
            QuestionPut = voting.QuestionPut,
            TotalVotedWeight = totalVotedWeight,
            Responses = responses,
            Decision = decision
        };

        return Ok(result);
    }

    [HttpPost("{id}/decision")]
    public async Task<ActionResult> SetVotingDecision(Guid id, [FromBody] string decision)
    {
        // 1. Найти голосование
        var voting = await _context.Votings
            .FirstOrDefaultAsync(v => v.Id == id);

        if (voting == null)
            return NotFound("Голосование не найдено");

        // 2. Проверить, завершено ли голосование
        // (используем ту же логику, что и в GetVotingResults)
        bool isCompleted = voting.IsCompleted
                        || voting.EndTime < DateTime.UtcNow
                        || voting.OwnersList.All(o => !string.IsNullOrEmpty(o.Response));

        if (!isCompleted)
        {
            return BadRequest("Голосование ещё активно, нельзя внести решение");
        }

        // 3. Проверить, что решение не пустое (опционально)
        if (string.IsNullOrWhiteSpace(decision))
        {
            return BadRequest("Решение не может быть пустым");
        }

        // 4. Обновить поле Decision
        voting.Decision = decision;

        // 5. Сохранить изменения
        await _context.SaveChangesAsync();

        return Ok("Решение внесено");
    }
}