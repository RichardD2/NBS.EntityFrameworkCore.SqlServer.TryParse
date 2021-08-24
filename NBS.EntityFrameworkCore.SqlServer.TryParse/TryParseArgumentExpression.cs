using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace NBS.EntityFrameworkCore.SqlServer
{
	internal sealed class TryParseArgumentExpression : SqlExpression
	{
		private readonly SqlExpression _sourceExpression;
		private readonly SqlFragmentExpression _asExpression;

		public TryParseArgumentExpression(Type type, SqlExpression sourceExpression, string sqlTypeName) : base(type, sourceExpression.TypeMapping)
		{
			_sourceExpression = sourceExpression ?? throw new ArgumentNullException(nameof(sourceExpression));
			_asExpression = new SqlFragmentExpression($" AS {sqlTypeName}");
		}

		private TryParseArgumentExpression(Type type, SqlExpression sourceExpression, SqlFragmentExpression asExpression) : base(type, sourceExpression.TypeMapping)
		{
			_sourceExpression = sourceExpression ?? throw new ArgumentNullException(nameof(sourceExpression));
			_asExpression = asExpression ?? throw new ArgumentNullException(nameof(asExpression));
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			var hash = new HashCode();
			hash.Add(base.GetHashCode());
			hash.Add(_sourceExpression);
			hash.Add(_asExpression);
			return hash.ToHashCode();
		}

		/// <inheritdoc />
		public override bool Equals(object obj)
			=> obj is TryParseArgumentExpression other
			&& Equals(other._sourceExpression, _sourceExpression)
			&& Equals(other._asExpression, _asExpression);

		/// <inheritdoc />
		protected override Expression VisitChildren(ExpressionVisitor visitor)
		{
			if (visitor is null) throw new ArgumentNullException(nameof(visitor));

			var newSource = (SqlExpression?)visitor.Visit(_sourceExpression) ?? _sourceExpression;
			var newAsExpression = (SqlFragmentExpression?)visitor.Visit(_asExpression) ?? _asExpression;
			if (Equals(newSource, _sourceExpression) && Equals(newAsExpression, _asExpression)) return this;
			return new TryParseArgumentExpression(Type, newSource, newAsExpression);
		}

		/// <inheritdoc />
		protected override Expression Accept(ExpressionVisitor visitor)
		{
			if (visitor is null) throw new ArgumentNullException(nameof(visitor));

			visitor.Visit(_sourceExpression);
			visitor.Visit(_asExpression);
			return this;
		}

		/// <inheritdoc />
		public override void Print(ExpressionPrinter expressionPrinter)
		{
			if (expressionPrinter is null) throw new ArgumentNullException(nameof(expressionPrinter));

			expressionPrinter.Visit(_sourceExpression);
			expressionPrinter.Visit(_asExpression);
		}
	}
}