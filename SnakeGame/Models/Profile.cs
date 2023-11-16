using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SnakeGame.Models;

public class Profile
{
    public int Id { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Highscore { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }
}

public class ProfileDto
{
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Highscore { get; set; }

    public ProfileDto(Profile profile)
    {
        Wins = profile.Wins;
        Losses = profile.Losses;
        Highscore = profile.Highscore;
    }
}
