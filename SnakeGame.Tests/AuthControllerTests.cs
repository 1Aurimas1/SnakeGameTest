using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SnakeGame.Controllers;
using SnakeGame.Data;
using SnakeGame.Models;

namespace SnakeGame.Tests;

public sealed class AuthControllerTests : IDisposable
{
    private readonly AuthController _sut;
    private readonly SqliteConnection _connection;
    private readonly DataContext _dataContext;

    public AuthControllerTests()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _dataContext = new DataContext(new DbContextOptionsBuilder<DataContext>()
            .UseSqlite(_connection)
            .Options);

        _dataContext.Database.Migrate();

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string?>("Jwt:Issuer", "https://localhost:8000"),
                new KeyValuePair<string, string?>("Jwt:Audience", "https://localhost:3000"),
                new KeyValuePair<string, string?>("Jwt:Authority", "https://localhost:8000"),
                new KeyValuePair<string, string?>("Jwt:Key", "This is development test key"),
            })
            .Build();

        _sut = new AuthController(configuration, _dataContext);
    }

    [Theory]
    [InlineData("mail")]
    [InlineData("mail.")]
    public async Task Register_ShouldReturnError_WhenEmailIsNotValid(string email)
    {
        // Arrange
        var request = new UserRegisterDto
        {
            Username = "Name",
            Password = "Password",
            Email = email,
            PasswordConfirmation = "Password",
        };

        // Act
        var response = await _sut.Register(request);

        // Assert
        response.Value.Should().BeNull();
        var result = response.Result.Should().BeAssignableTo<BadRequestObjectResult>().Which;
        result.StatusCode.Should().Be(400);
        var resultValues = result.Value.Should().BeAssignableTo<Dictionary<string, object>>().Which;
        resultValues.Should().HaveCount(1);
        resultValues.Should().ContainEquivalentOf(new KeyValuePair<string, object>("Email", new[] { "The email is not valid." }));
    }

    [Fact]
    public async Task Register_ShouldReturnError_WhenUsernameIsTaken()
    {
        // Arrange
        var request = new UserRegisterDto
        {
            Username = "Name",
            Password = "Password",
            Email = "mail@to.me",
            PasswordConfirmation = "Password",
        };
        await _dataContext.AddAsync(new User
        {
            Username = "Name",
            Email = "mail2@to.me",
            PasswordHash = "",
        });
        await _dataContext.SaveChangesAsync();

        // Act
        var response = await _sut.Register(request);

        // Assert
        response.Value.Should().BeNull();
        var result = response.Result.Should().BeAssignableTo<BadRequestObjectResult>().Which;
        result.StatusCode.Should().Be(400);
        var resultValues = result.Value.Should().BeAssignableTo<Dictionary<string, object>>().Which;
        resultValues.Should().HaveCount(1);
        resultValues.Should().ContainEquivalentOf(new KeyValuePair<string, object>("Username", new[] { "The username is already in use." }));
    }

    [Fact]
    public async Task Register_ShouldReturnError_WhenEmailIsTaken()
    {
        // Arrange
        var request = new UserRegisterDto
        {
            Username = "Name",
            Password = "Password",
            Email = "mail@to.me",
            PasswordConfirmation = "Password",
        };
        await _dataContext.AddAsync(new User
        {
            Username = "Name2",
            Email = "mail@to.me",
            PasswordHash = "",
        });
        await _dataContext.SaveChangesAsync();

        // Act
        var response = await _sut.Register(request);

        // Assert
        response.Value.Should().BeNull();
        var result = response.Result.Should().BeAssignableTo<BadRequestObjectResult>().Which;
        result.StatusCode.Should().Be(400);
        var resultValues = result.Value.Should().BeAssignableTo<Dictionary<string, object>>().Which;
        resultValues.Should().HaveCount(1);
        resultValues.Should().ContainEquivalentOf(new KeyValuePair<string, object>("Email", new[] { "The email is already in use." }));
    }

    [Fact]
    public async Task Register_ShouldCreateUser_WhenRegistrationDataIsValid()
    {
        // Arrange
        var request = new UserRegisterDto
        {
            Username = "Name",
            Password = "Password",
            Email = "mail@to.me",
            PasswordConfirmation = "Password",
        };

        // Act
        var response = await _sut.Register(request);

        // Assert
        response.Value.Should().BeNull();
        var result = response.Result.Should().BeAssignableTo<OkObjectResult>().Which;
        result.StatusCode.Should().Be(200);
        result.Value.Should().Be("User registered successfully.");

        var user = await _dataContext.Users.FirstAsync();
        user.Username.Should().Be("Name");
        user.Email.Should().Be("mail@to.me");
        user.PasswordHash.Should().Match(value => new PasswordHasher<UserDto>(null).VerifyHashedPassword(null!, value, "Password") == PasswordVerificationResult.Success);
    }

    [Fact]
    public async Task Login_ShouldLoginUser_WhenLoginDataIsValid()
    {
        // Arrange
        await _dataContext.AddAsync(new User
        {
            Username = "Name",
            Email = "mail@to.me",
            PasswordHash = new PasswordHasher<UserDto>(
                    Options.Create(new PasswordHasherOptions { CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV2 }))
                .HashPassword(null!, "Password"),
        });
        await _dataContext.SaveChangesAsync();

        var request = new UserDto
        {
            Username = "Name",
            Password = "Password",
        };

        // Act
        var response = await _sut.Login(request);

        // Assert
        response.Value.Should().BeNull();
        var result = response.Result.Should().BeAssignableTo<OkObjectResult>().Which;
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeAssignableTo<string>().Which.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_ShouldReturnError_WhenUserNotFound()
    {
        // Arrange
        var request = new UserDto
        {
            Username = "Name",
            Password = "Password",
        };

        // Act
        var response = await _sut.Login(request);

        // Assert
        response.Value.Should().BeNull();
        var result = response.Result.Should().BeAssignableTo<BadRequestObjectResult>().Which;
        result.StatusCode.Should().Be(400);
        var resultValues = result.Value.Should().BeAssignableTo<Dictionary<string, object>>().Which;
        resultValues.Should().HaveCount(1);
        resultValues.Should().ContainEquivalentOf(new KeyValuePair<string, object>("Username", new[] { "User not registered" }));
    }

    [Fact]
    public async Task Login_ShouldReturnError_WhenPasswordIsInvalid()
    {
        // Arrange
        await _dataContext.AddAsync(new User
        {
            Username = "Name",
            Email = "mail@to.me",
            PasswordHash = new PasswordHasher<UserDto>().HashPassword(null!, "Password"),
        });
        await _dataContext.SaveChangesAsync();

        var request = new UserDto
        {
            Username = "Name",
            Password = "Password2",
        };

        // Act
        var response = await _sut.Login(request);

        // Assert
        response.Value.Should().BeNull();
        var result = response.Result.Should().BeAssignableTo<BadRequestObjectResult>().Which;
        result.StatusCode.Should().Be(400);
        var resultValues = result.Value.Should().BeAssignableTo<Dictionary<string, object>>().Which;
        resultValues.Should().HaveCount(1);
        resultValues.Should().ContainEquivalentOf(new KeyValuePair<string, object>("Username", new[] { "Incorrect login information" }));
    }

    public void Dispose()
    {
        _connection.Dispose();
        _dataContext.Dispose();
    }
}
