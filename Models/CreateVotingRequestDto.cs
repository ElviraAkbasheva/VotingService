namespace VotingService.Models;

public class CreateVotingRequestDto
{
    public string QuestionPut { get; set; } = string.Empty;
    public List<string> ResponseOptions { get; set; } = new();
    public List<long> HouseIds { get; set; } = new();
}