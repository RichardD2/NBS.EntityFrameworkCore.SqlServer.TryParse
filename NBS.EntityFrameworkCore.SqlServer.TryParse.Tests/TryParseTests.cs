using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NBS.EntityFrameworkCore.SqlServer.Tests
{
	[TestClass]
	public class TryParseTests : IntegrationTestBase
	{
		[TestMethod]
		public void CanParseInt32()
		{
			var values = DbContext!.TestEntities.Select(e => new { e.Value, ValueInt32 = TryParse.Int32(e.Value) }).ToList();
			Assert.AreNotEqual(0, values.Count);

			foreach (var entity in values)
			{
				int? expected = int.TryParse(entity.Value, out var x) ? x : null;
				Assert.AreEqual(expected, entity.ValueInt32, $"Failed to parse '{entity.Value}' as Int32");
			}
		}

		[TestMethod]
		public void CanParseInt64()
		{
			var values = DbContext!.TestEntities.Select(e => new { e.Value, ValueInt64 = TryParse.Int64(e.Value) }).ToList();
			Assert.AreNotEqual(0, values.Count);

			foreach (var entity in values)
			{
				long? expected = long.TryParse(entity.Value, out var x) ? x : null;
				Assert.AreEqual(expected, entity.ValueInt64, $"Failed to parse '{entity.Value}' as Int32");
			}
		}
	}
}