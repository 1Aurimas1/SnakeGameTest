using FluentAssertions;
using Meziantou.Extensions.Logging.Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using SnakeGame.Data;
using SnakeGame.Models;
using System.Net.Http.Json;
using Testcontainers.PostgreSql;
using Xunit.Abstractions;

namespace SnakeGame.Tests.Integration;

public class ApplicationTests : IAsyncLifetime
{
    private HttpClient _client = null!;

    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithUsername("root")
        .WithPassword("root")
        .WithDatabase("snakedb")
        .Build();

    private readonly ITestOutputHelper _testOutputHelper;

    public ApplicationTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task MethodName_ShouldDoWhat_WhenWhat()
    {
        // Arrange
        var request = new UserRegisterDto
        {
            Username = "name",
            Password = "password",
            Email = "mail@to.me",
            PasswordConfirmation = "password",
        };

        // Act
        var response = await _client.PostAsync("/api/auth/register", JsonContent.Create(request));

        // Assert
        response.EnsureSuccessStatusCode();
        response.Content.Headers.ContentType!.ToString().Should().Be("text/plain; charset=utf-8");
        (await response.Content.ReadAsStringAsync()).Should().Be("User registered successfully.");
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        var waf = new WebApplicationFactory<IApiMarker>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureLogging(x =>
                {
                    x.ClearProviders();

                    x.Services.AddSingleton<ILoggerProvider>(new XUnitLoggerProvider(_testOutputHelper));
                });

                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<DbContextOptions<DataContext>>();
                    services.RemoveAll<DataContext>();

                    services.AddDbContext<DataContext>(options => options.UseNpgsql(_dbContainer.GetConnectionString()));
                });
            });

        _client = waf.CreateClient();

        await using var scope = waf.Services.GetRequiredService<IServiceScopeFactory>().CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<DataContext>().Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
        _client.Dispose();
    }
}
