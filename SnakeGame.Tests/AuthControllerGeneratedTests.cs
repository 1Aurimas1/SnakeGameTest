using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SnakeGame.Controllers;
using SnakeGame.Data;
using SnakeGame.Models;

namespace SnakeGame.Tests;

public class AuthControllerGeneratedTests
{
    [Fact]
    public async Task Register_ValidUser_ReturnsOkResult()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryAuthTestDatabase")
            .Options;
        using var context = new DataContext(options);
        context.Database.EnsureDeleted();

        var authController = new AuthController(configuration, context);

        var userDto = new UserRegisterDto
        {
            Username = "testuser",
            Password = "testpassword",
            Email = "test@example.com",
            PasswordConfirmation = "testpassword"
        };
        
        // Act
        var result = await authController.Register(userDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal("User registered successfully.", okResult.Value);
    }

    [Fact]
    public async Task Register_InvalidUser_ReturnsBadRequest()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryAuthTestDatabase")
            .Options;
        using var context = new DataContext(options);
        context.Database.EnsureDeleted();

        var authController = new AuthController(configuration, context);

        // Seed the in-memory database with a test user
        var testUser = new User
        {
            Username = "testuser",
            PasswordHash = new PasswordHasher<UserDto>().HashPassword(
                new UserDto { Username = "testuser", Password = "testpassword" },
                "testpassword"
            )
        };
        context.Users.Add(testUser);
        context.SaveChanges();

        // Invalid user with missing required fields
        var userDto = new UserRegisterDto
        {
            Username = "",
            Password = "",
            Email = "",
            PasswordConfirmation = ""
        };

        authController.ModelState.AddModelError("Username", "The username is required.");
        authController.ModelState.AddModelError("Email", "The email is required.");

        // Act
        var result = await authController.Register(userDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.IsType<SerializableError>(badRequestResult.Value);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkResultWithToken()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryAuthTestDatabase")
            .Options;

        using var context = new DataContext(options);
        context.Database.EnsureDeleted();

        var authController = new AuthController(configuration, context);

        // Seed the in-memory database with a test user
        var testUser = new User
        {
            Username = "testuser",
            PasswordHash = new PasswordHasher<UserDto>().HashPassword(
                new UserDto { Username = "testuser", Password = "testpassword" },
                "testpassword"
            )
        };
        context.Users.Add(testUser);
        context.SaveChanges();

        var userDto = new UserDto { Username = "testuser", Password = "testpassword" };

        // Act
        var result = await authController.Login(userDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var token = Assert.IsType<string>(okResult.Value);
        Assert.NotNull(token);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsBadRequest()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryAuthTestDatabase")
            .Options;

        using var context = new DataContext(options); // No user is seeded in the in-memory database
        context.Database.EnsureDeleted();

        var authController = new AuthController(configuration, context);

        var userDto = new UserDto { Username = "nonexistentuser", Password = "wrongpassword" };

        // Act
        var result = await authController.Login(userDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var modelState = Assert.IsType<SerializableError>(badRequestResult.Value);
        Assert.True(modelState.ContainsKey(nameof(UserDto.Username)));
        Assert.Equal("User not registered", ((string[])modelState[nameof(UserDto.Username)])[0]);
    }
}
