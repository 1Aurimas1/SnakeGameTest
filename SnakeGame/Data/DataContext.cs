global using Microsoft.EntityFrameworkCore;
using SnakeGame.Models;

namespace SnakeGame.Data;

public interface IDataContext
{
	DbSet<User> Users { get; set; }
	DbSet<Profile> Profiles { get; set; }

	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public class DataContext : DbContext, IDataContext
{
	public DataContext(DbContextOptions<DataContext> options)
		: base(options)
	{

	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		//base.OnModelCreating(modelBuilder);
		modelBuilder.Entity<User>()
			.HasIndex(u => u.Username)
			.IsUnique();

		modelBuilder.Entity<User>()
			.HasIndex(u => u.Email)
			.IsUnique();

		modelBuilder.Entity<User>()
			.HasOne(u => u.Profile)
			.WithOne(p => p.User)
			.HasForeignKey<Profile>(p => p.UserId)
			.OnDelete(DeleteBehavior.Cascade);
	}

	public DbSet<User> Users { get; set; }
	public DbSet<Profile> Profiles { get; set; }

	public new async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		return await base.SaveChangesAsync(cancellationToken);
	}
}
