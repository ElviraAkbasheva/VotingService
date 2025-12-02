using VotingService.Models;

namespace VotingService.Services;

public class MockApartmentServiceClient : IApartmentServiceClient
{
    public Task<List<ApartmentResponseDto>> GetApartmentsByHouseIdAsync(long houseId)
    {
        var apartments = new List<ApartmentResponseDto>
        {
            new()
            {
                Id = 101,
                Number = 42,
                NumbersOfRooms = 2,
                ResidentialArea = 45.0m,
                TotalArea = 55.0m,
                Floor = 5,
                EntranceNumber = 3,
                HouseId = houseId,
                Users = new List<ApartmentUserResponseDto>
                {
                    new()
                    {
                        UserId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                        Share = 1.0m,
                        UserDetails = new UserDto
                        {
                            Id = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                            FirstName = "Иван",
                            LastName = "Иванов",
                            Patronymic = "Иванович",
                            PhoneNumber = "+79001112233",
                            Email = "ivan@example.com"
                        },
                        Statuses = new List<StatusDto>
                        {
                            new() { Id = 1, Name = "Собственник" }
                        }
                    }
                }
            },
            new()
            {
                Id = 102,
                Number = 15,
                NumbersOfRooms = 3,
                ResidentialArea = 60.0m,
                TotalArea = 70.0m,
                Floor = 2,
                EntranceNumber = 1,
                HouseId = houseId,
                Users = new List<ApartmentUserResponseDto>
                {
                    new()
                    {
                        UserId = Guid.Parse("a1b2c3d4-e5f6-4890-a1b2-c3d4e5f67890"),
                        Share = 0.5m,
                        UserDetails = new UserDto
                        {
                            Id = Guid.Parse("a1b2c3d4-e5f6-4890-a1b2-c3d4e5f67890"),
                            FirstName = "Петр",
                            LastName = "Петров",
                            Patronymic = "Петрович",
                            PhoneNumber = "+79002223344",
                            Email = "petr@example.com"
                        },
                        Statuses = new List<StatusDto>
                        {
                            new() { Id = 1, Name = "Собственник" },
                            new() { Id = 2, Name = "Прописан" }
                        }
                    },
                    new()
                    {
                        UserId = Guid.Parse("b2c3d4e5-f6a7-4901-b2c3-d4e5f6a78901"),
                        Share = 0.5m,
                        UserDetails = new UserDto
                        {
                            Id = Guid.Parse("b2c3d4e5-f6a7-4901-b2c3-d4e5f6a78901"),
                            FirstName = "Анна",
                            LastName = "Сидорова",
                            Patronymic = "Сергеевна",
                            PhoneNumber = "+79003334455",
                            Email = "anna@example.com"
                        },
                        Statuses = new List<StatusDto>
                        {
                            new() { Id = 1, Name = "Собственник" }
                        }
                    }
                }
            }
        };

        return Task.FromResult(apartments);
    }
}