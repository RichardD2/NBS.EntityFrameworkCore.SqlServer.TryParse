using System;
using Microsoft.EntityFrameworkCore;

namespace NBS.EntityFrameworkCore.SqlServer.Tests
{
	public class TestDbContext : DbContext
	{
		public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
		{
		}

		public DbSet<TestEntity> TestEntities => Set<TestEntity>();

		/// <inheritdoc />
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			TryParse.Register(modelBuilder);

			var entity = modelBuilder.Entity<TestEntity>();
			entity.Property(t => t.Id).ValueGeneratedOnAdd();

			entity.HasData(
				new TestEntity(1, "1"), 
				new TestEntity(2, "2"), 
				new TestEntity(3, "3"), 
				new TestEntity(4, "Should not parse"));
		}
	}
}