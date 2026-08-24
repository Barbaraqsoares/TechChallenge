namespace TechChallenge.Domain.Models.UserGame;

public class AddGameToUserRequest
{
    public int GameId { get; set; }
    public int UserId { get; set; }
}