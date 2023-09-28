using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NBS.EntityFrameworkCore.SqlServer.Tests
{
	public sealed class MsTestLogger : ILogger, IDisposable
	{
		public MsTestLogger(string categoryName, TestContext testContext)
		{
			CategoryName = categoryName;
			TestContext = testContext;
		}

		private string CategoryName { get; }

		private TestContext TestContext { get; }

		/// <inheritdoc />
		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
		{
			TestContext.WriteLine("[{0}] {1}: {2}", CategoryName, logLevel, formatter(state, exception));
		}

		/// <inheritdoc />
		public bool IsEnabled(LogLevel logLevel) => true;

		/// <inheritdoc />
		public IDisposable? BeginScope<TState>(TState state) where TState : notnull => this;

		/// <inheritdoc />
		public void Dispose()
		{
		}
	}

	public sealed class MsTestLoggerProvider : ILoggerProvider
	{
		private readonly ConcurrentDictionary<string, MsTestLogger> _loggers = new(StringComparer.Ordinal);

		public MsTestLoggerProvider(TestContext testContext)
		{
			TestContext = testContext;
		}

		private TestContext TestContext { get; }

		/// <inheritdoc />
		public ILogger CreateLogger(string categoryName) => _loggers.GetOrAdd(categoryName, static (key, context) => new MsTestLogger(key, context), TestContext);

		/// <inheritdoc />
		public void Dispose()
		{
		}
	}

	public static class MsTestLoggerExtensions
	{
		public static ILoggingBuilder AddMsTest(this ILoggingBuilder builder, TestContext testContext)
		{
			ArgumentNullException.ThrowIfNull(builder);
			ArgumentNullException.ThrowIfNull(testContext);
			builder.Services.AddSingleton<ILoggerProvider>(new MsTestLoggerProvider(testContext));
			return builder;
		}
	}
}