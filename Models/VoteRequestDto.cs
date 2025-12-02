namespace VotingService.Models;

public class VoteRequestDto
{
    public Guid UserId { get; set; }
    public long ApartmentId { get; set; }
    public string Response { get; set; } = string.Empty;
}