using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SnakeGame.Models;

public interface IUserDto
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class UserDto : IUserDto
{
    [Required]
    [JsonPropertyName("username")]
    [MaxLength(20, ErrorMessage = "Username should not exceed 20 characters")]
    public required string Username { get; set; }
    [Required]
    [JsonPropertyName("password")]
    [MinLength(6, ErrorMessage = "Password should contain atleast 6 characters")]
    public required string Password { get; set; }
}

public class UserRegisterDto : UserDto
{
    [Required]
    [JsonPropertyName("email")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public required string Email { get; set; }
    [Required]
    [JsonPropertyName("passwordConfirmation")]
    [Compare("Password", ErrorMessage = "Password and confirmation do not match.")]
    public required string PasswordConfirmation { get; set; }
}

