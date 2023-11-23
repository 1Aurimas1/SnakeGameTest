using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using SnakeGame.Data;
using Testcontainers.PostgreSql;

namespace SnakeGame.Tests;

public class MigrationsTests
{
    [Fact]
    public async Task DataContext_ShouldUpAndDown()
    {
        // Arrange
        var dbContainer = new PostgreSqlBuilder()
            .WithUsername("root")
            .WithPassword("root")
            .WithDatabase("snakedb")
            .Build();
        await dbContainer.StartAsync();

        var dataContextOptionsBuilder = new DbContextOptionsBuilder<DataContext>();
        dataContextOptionsBuilder.UseNpgsql(dbContainer.GetConnectionString());
        await using var dataContext = new DataContext(dataContextOptionsBuilder.Options);

        // Act
        // Migrate Up
        await dataContext.Database.MigrateAsync();

        // Migrate Down
        var migrator = dataContext.GetInfrastructure().GetService(typeof(IMigrator)) as IMigrator;
        await migrator!.MigrateAsync("20230815160954_InitialCreate");

        // Assert
        Assert.True(true);
    }
}
