using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SnakeGame.Controllers;
using SnakeGame.Data;
using SnakeGame.Models;

namespace SnakeGame.Tests;

public class ProfileControllerGeneratedTests
{
    [Fact]
    public async Task Get_AuthenticatedUser_ReturnsOkResultWithProfileDto()
    {
        // Arrange
        var userId = "1";
        var user = new ClaimsPrincipal(
            new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, userId) })
        );

        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryProfileTestDatabase")
            .Options;

        using var context = new DataContext(options);
        context.Database.EnsureDeleted();
        // Seed the in-memory database with a test profile
        var testProfile = new Profile
        {
            Wins = 10,
            Losses = 5,
            Highscore = 100,
            UserId = int.Parse(userId)
        };
        context.Profiles.Add(testProfile);
        context.SaveChanges();

        var controller = new ProfileController(context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            }
        };

        // Act
        var result = await controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var profileDto = Assert.IsType<ProfileDto>(okResult.Value);
        Assert.Equal(10, profileDto.Wins);
        Assert.Equal(5, profileDto.Losses);
        Assert.Equal(100, profileDto.Highscore);
    }

    [Fact]
    public async Task Get_UnauthenticatedUser_ReturnsBadRequest()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryProfileTestDatabase")
            .Options;

        using var context = new DataContext(options);
        context.Database.EnsureDeleted();

        var controller = new ProfileController(context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = null }
            }
        };

        // Act
        var result = await controller.Get();

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Error retrieving user id", badRequestResult.Value);
    }

    [Fact]
    public async Task Get_UserProfileNotFound_ReturnsBadRequest()
    {
        // Arrange
        var userId = "-1337";
        var user = new ClaimsPrincipal(
            new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, userId) })
        );

        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryProfileTestDatabase")
            .Options;

        using var context = new DataContext(options);
        context.Database.EnsureDeleted();

        var controller = new ProfileController(context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            }
        };

        // Act
        var result = await controller.Get();

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("User profile not found", badRequestResult.Value);
    }
}
