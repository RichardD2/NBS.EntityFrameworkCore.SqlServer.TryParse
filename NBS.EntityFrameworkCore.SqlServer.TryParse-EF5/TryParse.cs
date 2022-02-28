using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace NBS.EntityFrameworkCore.SqlServer
{
	/// <summary>
	/// Methods for calling <a href="https://docs.microsoft.com/en-us/sql/t-sql/functions/try-parse-transact-sql" target="_blank">TRY_PARSE</a> from EF Core.
	/// </summary>
	/// <example>
	/// <para>Register the extension in the <c>OnModelCreating</c> method:</para>
	/// <code>
	/// protected override void OnModelCreating(ModelBuilder modelBuilder)
	/// {
	///     base.OnModelCreating(modelBuilder);
	///     TryParse.Register(modelBuilder);
	/// }
	/// </code>
	/// <para>You can then call the functions as part of a query against this context:</para>
	/// <code>
	/// var results = context.SomeSet.Select(e =&gt; new { e.Id, e.Name, ValueInt32 = TryParse.Int32(e.Value) }).ToList();
	/// </code>
	/// <para>This will be translated into a suitable SQL query:</para>
	/// <code>
	/// SELECT Id, Name, TRY_PARSE(Value As int) As ValueInt32 FROM SomeSet
	/// </code>
	/// </example>
	public static class TryParse
	{
		private static readonly bool[] ArgumentsPropagateNullability = { true };

		/// <summary>
		/// Registers the <c>TRY_PARSE</c> functions.
		/// </summary>
		/// <param name="modelBuilder">
		/// The <see cref="ModelBuilder"/>.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="modelBuilder"/> is <see langword="null"/>.
		/// </exception>
		public static void Register(ModelBuilder modelBuilder)
		{
			if (modelBuilder is null) throw new ArgumentNullException(nameof(modelBuilder));

			foreach (var dbFunc in typeof(TryParse).GetMethods(BindingFlags.Public | BindingFlags.Static))
			{
				var attribute = dbFunc.GetCustomAttribute<SqlTypeNameAttribute>();
				if (attribute is null) continue;

				modelBuilder.HasDbFunction(dbFunc).HasTranslation(args =>
				{
					var newArgs = args.ToList();
					newArgs[0] = new TryParseArgumentExpression(dbFunc.ReturnType, newArgs[0], attribute.SqlTypeName);
					return new SqlFunctionExpression("TRY_PARSE", newArgs, true, ArgumentsPropagateNullability, dbFunc.ReturnType, null);
				});
			}
		}

		/// <summary>
		/// Attempts to parse the string as a byte.
		/// </summary>
		/// <param name="value">The string value to parse.</param>
		/// <returns>The parsed value, if available; otherwise, <see langword="null"/></returns>
		[SqlTypeName("tinyint")]
		public static byte? Byte(string value) => byte.TryParse(value, out var result) ? result : null;

		/// <summary>
		/// Attempts to parse the string as a 16-bit integer.
		/// </summary>
		/// <param name="value">The string value to parse.</param>
		/// <returns>The parsed value, if available; otherwise, <see langword="null"/></returns>
		[SqlTypeName("smallint")]
		public static short? Int16(string value) => short.TryParse(value, out var result) ? result : null;

		/// <summary>
		/// Attempts to parse the string as a 32-bit integer.
		/// </summary>
		/// <param name="value">The string value to parse.</param>
		/// <returns>The parsed value, if available; otherwise, <see langword="null"/></returns>
		[SqlTypeName("int")]
		public static int? Int32(string value) => int.TryParse(value, out var result) ? result : null;

		/// <summary>
		/// Attempts to parse the string as a 64-bit integer.
		/// </summary>
		/// <param name="value">The string value to parse.</param>
		/// <returns>The parsed value, if available; otherwise, <see langword="null"/></returns>
		[SqlTypeName("bigint")]
		public static long? Int64(string value) => long.TryParse(value, out var result) ? result : null;

		/// <summary>
		/// Attempts to parse the string as a decimal.
		/// </summary>
		/// <param name="value">The string value to parse.</param>
		/// <returns>The parsed value, if available; otherwise, <see langword="null"/></returns>
		[SqlTypeName("decimal")]
		public static decimal? Decimal(string value) => decimal.TryParse(value, out var result) ? result : null;

		/// <summary>
		/// Attempts to parse the string as a double-precision floating-point number.
		/// </summary>
		/// <param name="value">The string value to parse.</param>
		/// <returns>The parsed value, if available; otherwise, <see langword="null"/></returns>
		[SqlTypeName("float")]
		public static double? Double(string value) => double.TryParse(value, out var result) ? result : null;

		/// <summary>
		/// Attempts to parse the string as a single-precision floating-point number.
		/// </summary>
		/// <param name="value">The string value to parse.</param>
		/// <returns>The parsed value, if available; otherwise, <see langword="null"/></returns>
		[SqlTypeName("real")]
		public static float? Single(string value) => float.TryParse(value, out var result) ? result : null;

		/// <summary>
		/// Attempts to parse the string as a date.
		/// </summary>
		/// <param name="value">The string value to parse.</param>
		/// <returns>The parsed value, if available; otherwise, <see langword="null"/></returns>
		[SqlTypeName("date")]
		public static DateTime? Date(string value) => System.DateTime.TryParse(value, out var result) ? result : null;

		/// <summary>
		/// Attempts to parse the string as a <see cref="System.DateTime"/>.
		/// </summary>
		/// <param name="value">The string value to parse.</param>
		/// <returns>The parsed value, if available; otherwise, <see langword="null"/></returns>
		[SqlTypeName("datetime2")]
		public static DateTime? DateTime(string value) => System.DateTime.TryParse(value, out var result) ? result : null;

		/// <summary>
		/// Attempts to parse the string as a <see cref="System.DateTimeOffset"/>.
		/// </summary>
		/// <param name="value">The string value to parse.</param>
		/// <returns>The parsed value, if available; otherwise, <see langword="null"/></returns>
		[SqlTypeName("datetimeoffset")]
		public static DateTimeOffset? DateTimeOffset(string value) => System.DateTimeOffset.TryParse(value, out var result) ? result : null;

		/// <summary>
		/// Attempts to parse the string as a time.
		/// </summary>
		/// <param name="value">The string value to parse.</param>
		/// <returns>The parsed value, if available; otherwise, <see langword="null"/></returns>
		[SqlTypeName("time")]
		public static TimeSpan? Time(string value) => TimeSpan.TryParse(value, out var result) ? result : null;
	}
}