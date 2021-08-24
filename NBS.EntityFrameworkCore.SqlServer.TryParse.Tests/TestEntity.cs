using System;

namespace NBS.EntityFrameworkCore.SqlServer.Tests
{
	public class TestEntity
	{
		public TestEntity(int id, string value)
		{
			Id = id;
			Value = value;
		}

		public int Id { get; private set; }
		public string Value { get; private set; }
	}
}