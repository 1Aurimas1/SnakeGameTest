using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SnakeGame.Models;

public class User
{
    public int Id { get; set; }
    [Required]
    [JsonPropertyName("username")]
    [MaxLength(20, ErrorMessage = "Username should not exceed 20 characters")]
    public string Username { get; set; }
    [Required]
    [JsonPropertyName("email")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }
    [Required]
    [JsonPropertyName("password")]
    [MinLength(6, ErrorMessage = "Password should contain atleast 6 characters")]
    public string PasswordHash { get; set; }
    [Required]
    [JsonPropertyName("profile")]
    public Profile Profile { get; set; } = new Profile();

    public User()
    {
        Username = string.Empty;
        Email = string.Empty;
        PasswordHash = string.Empty;
    }

    public User(UserRegisterDto user)
    {
        Username = user.Username;
        Email = user.Email;
        PasswordHash = user.Password;
    }
}
