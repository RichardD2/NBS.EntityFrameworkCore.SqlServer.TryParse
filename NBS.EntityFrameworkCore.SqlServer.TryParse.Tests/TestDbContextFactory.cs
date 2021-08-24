using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NBS.EntityFrameworkCore.SqlServer.Tests
{
	public class TestDbContextFactory : IDesignTimeDbContextFactory<TestDbContext>
	{
		/// <inheritdoc />
		public TestDbContext CreateDbContext(string[] args)
		{
			var optionsBuilder = new DbContextOptionsBuilder<TestDbContext>();
			optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=NBS.EntityFrameworkCore.SqlServer.Tests;Trusted_Connection=True;");
			return new TestDbContext(optionsBuilder.Options);
		}
	}
}