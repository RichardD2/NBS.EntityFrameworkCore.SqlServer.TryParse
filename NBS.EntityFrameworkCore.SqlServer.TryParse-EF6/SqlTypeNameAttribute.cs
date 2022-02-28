using System;

namespace NBS.EntityFrameworkCore.SqlServer
{
	[AttributeUsage(AttributeTargets.Method)]
	internal sealed class SqlTypeNameAttribute : Attribute
	{
		public SqlTypeNameAttribute(string sqlTypeName)
		{
			if (string.IsNullOrEmpty(sqlTypeName)) throw new ArgumentNullException(nameof(sqlTypeName));
			SqlTypeName = sqlTypeName;
		}

		public string SqlTypeName { get; }
	}
}