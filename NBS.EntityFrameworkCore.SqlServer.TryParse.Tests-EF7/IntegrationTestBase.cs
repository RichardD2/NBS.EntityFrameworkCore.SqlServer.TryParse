using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NBS.EntityFrameworkCore.SqlServer.Tests
{
	public abstract class IntegrationTestBase
	{
		protected TestDbContext? DbContext { get; private set; }

		[TestInitialize]
		public void SetUp()
		{
			var dbContextFactory = new TestDbContextFactory();
			DbContext = dbContextFactory.CreateDbContext(Array.Empty<string>());
			DbContext.Database.EnsureCreated();
		}

		[TestCleanup]
		public void TearDown()
		{
			if (DbContext is not null)
			{
				DbContext.Database.EnsureDeleted();
				DbContext.Dispose();
				DbContext = null;
			}
		}
	}
}