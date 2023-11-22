using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SnakeGame.Models;

namespace SnakeGame.Controllers;

[Route("/api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IDataContext _context;

    public AuthController(IConfiguration configuration, IDataContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register(UserRegisterDto userDto)
    {
        if (!IsValidEmail(userDto.Email))
            ModelState.AddModelError(nameof(userDto.Email), "The email is not valid.");

        if (_context.Users.Any(u => u.Username == userDto.Username))
            ModelState.AddModelError(nameof(userDto.Username), "The username is already in use.");

        if (_context.Users.Any(u => u.Email == userDto.Email))
            ModelState.AddModelError(nameof(userDto.Email), "The email is already in use.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var hasher = new PasswordHasher<UserDto>();
        var hash = hasher.HashPassword(userDto, userDto.Password);
        userDto.Password = hash;

        _context.Users.Add(new User(userDto));
        await _context.SaveChangesAsync();

        return Ok("User registered successfully.");
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(UserDto userDto)
    {
        var u = await _context.Users.FirstOrDefaultAsync(u => u.Username == userDto.Username);

        if (u == null)
            ModelState.AddModelError(nameof(userDto.Username), "User not registered");
        else
        {
            var hasher = new PasswordHasher<UserDto>();
            var verificationResult = hasher.VerifyHashedPassword(userDto, u!.PasswordHash, userDto.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
                ModelState.AddModelError(nameof(userDto.Username), "Incorrect login information");
            else if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
            {
                System.Console.WriteLine("Deprecated algorithm was used. Rehashing password...");
                var newHash = hasher.HashPassword(userDto, userDto.Password);

                u.PasswordHash = newHash;
                await _context.SaveChangesAsync();
            }
        }

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        string token = CreateToken(u!);

        return Ok(token);
    }

    private string CreateToken(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");

        var claims = new List<Claim> {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds,
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"]
                );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }

    private bool IsValidEmail(string email)
    {
        var trimmedEmail = email.Trim();

        if (trimmedEmail.EndsWith(".")) return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == trimmedEmail;
        }
        catch
        {
            return false;
        }
    }
}
