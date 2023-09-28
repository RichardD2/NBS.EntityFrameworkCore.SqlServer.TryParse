using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NBS.EntityFrameworkCore.SqlServer.Tests
{
	public abstract class IntegrationTestBase
	{
		public TestContext TestContext { get; set; }

		protected IServiceProvider ServiceProvider { get; private set; }

		protected TestDbContext? DbContext { get; private set; }

		[TestInitialize]
		public void SetUp()
		{
			ServiceCollection services = new();
			ConfigureServices(services);

			ServiceProvider = services.BuildServiceProvider();
			DbContext = ServiceProvider.GetRequiredService<TestDbContext>();
			DbContext.Database.EnsureCreated();
		}

		protected virtual void ConfigureServices(IServiceCollection services)
		{
			services.AddLogging(builder => builder.AddMsTest(TestContext));
			services.AddDbContext<TestDbContext>((sp, options) =>
			{
				options.UseSqlServer(TestDbContext.DefaultConnectionString, b =>
				{
					b.UseRelationalNulls();
				});

				options.UseLoggerFactory(sp.GetRequiredService<ILoggerFactory>());
				options.EnableDetailedErrors();
				options.EnableSensitiveDataLogging();
			});
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

			if (ServiceProvider is IDisposable sp)
			{
				sp.Dispose();
			}
		}
	}
}