using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnakeGame.Models;
using System.Security.Claims;

namespace SnakeGame.Controllers;

[Authorize]
[Route("/api/[controller]")]
[ApiController]
public class ProfileController : ControllerBase
{
    private readonly DataContext _context;

    public ProfileController(DataContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<Profile>> Get()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
            return BadRequest("Error retrieving user id");

        var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == int.Parse(userId));

        if (profile == null)
            return BadRequest("User profile not found");

        var dto = new ProfileDto(profile);

        return Ok(dto);
    }
}
