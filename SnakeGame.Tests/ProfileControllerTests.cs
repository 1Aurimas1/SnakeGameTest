using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SnakeGame.Controllers;
using SnakeGame.Data;
using SnakeGame.Models;
using System.Security.Claims;

namespace SnakeGame.Tests;

public sealed class ProfileControllerTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DataContext _dataContext;
    private readonly ProfileController _sut;

    public ProfileControllerTests()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _dataContext = new DataContext(new DbContextOptionsBuilder<DataContext>()
            .UseSqlite(_connection)
            .Options);

        _dataContext.Database.Migrate();

        _sut = new ProfileController(_dataContext);
    }

    [Fact]
    public async Task Get_ShouldReturnError_WhenUserWasNotFound()
    {
        // Arrange
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity()),
            },
        };

        // Act
        var response = await _sut.Get();

        // Assert
        response.Value.Should().BeNull();
        var result = response.Result.Should().BeAssignableTo<BadRequestObjectResult>().Which;
        result.StatusCode.Should().Be(400);
        result.Value.Should().BeAssignableTo<string>().Which.Should().Be("Error retrieving user id");
    }

    [Fact]
    public async Task Get_ShouldReturnError_WhenProfileWasNotFound()
    {
        // Arrange
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new []
                {
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                })),
            },
        };

        // Act
        var response = await _sut.Get();

        // Assert
        response.Value.Should().BeNull();
        var result = response.Result.Should().BeAssignableTo<BadRequestObjectResult>().Which;
        result.StatusCode.Should().Be(400);
        result.Value.Should().BeAssignableTo<string>().Which.Should().Be("User profile not found");
    }

    [Fact]
    public async Task Get_ShouldReturnProfile_WhenProfileExists()
    {
        // Arrange
        var user = await _dataContext.AddAsync(new User
        {
            Username = "Name",
            Email = "mail@to.me",
            PasswordHash = "",
            Profile = new Profile
            {
                Wins = 1,
                Losses = 2,
                Highscore = 3,
            },
        });
        await _dataContext.SaveChangesAsync();

        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new []
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Entity.Id.ToString()),
                })),
            },
        };

        // Act
        var response = await _sut.Get();

        // Assert
        response.Value.Should().BeNull();
        var result = response.Result.Should().BeAssignableTo<OkObjectResult>().Which;
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeAssignableTo<ProfileDto>().Which.Should().BeEquivalentTo(new ProfileDto(new Profile
        {
            Wins = 1,
            Losses = 2,
            Highscore = 3,
        }));
    }

    public void Dispose()
    {
        _connection.Dispose();
        _dataContext.Dispose();
    }
}
