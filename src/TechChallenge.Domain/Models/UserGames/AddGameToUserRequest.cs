namespace TechChallenge.Domain.Models.User;

public class AddGameToUserRequest
{
    public int GameId { get; set; }
    public int UserId { get; set; }

}