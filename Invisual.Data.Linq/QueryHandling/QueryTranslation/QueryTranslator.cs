using Invisual.Data.Linq.QueryHandling.QueryTranslation.SqlSegmentTranslators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Invisual.Data.Linq.QueryHandling.QueryTranslation
{
	public class QueryTranslator : ExpressionVisitor, IQueryTranslator
	{
		private readonly List<Expression> _whereClauses;

		public QueryTranslator(Expression expression)
		{
			if (expression == null)
				throw new ArgumentNullException("expression");

			_whereClauses = new List<Expression>();

			Visit(expression);
		}

		public string GetSql()
		{
			if (!_whereClauses.Any())
				return string.Empty;

			var searchConditions = _whereClauses.Select(c => new WhereTranslator(c).GetSql()).ToList();

			var sql = new StringBuilder();
			sql.AppendLine("WHERE");
			sql.AppendLine(searchConditions[0]);

			for (var i = 1; i < searchConditions.Count(); i++)
				sql.AppendFormat("AND {0}", searchConditions[i]);

			return sql.ToString();
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			if (node.Method.DeclaringType != typeof(Queryable))
				throw new NotSupportedException("The type for the query operator is not Queryable!");

			switch (node.Method.Name)
			{
				case "Where":
					_whereClauses.Add(node.Arguments[1]);

					break;

				default:
					return base.VisitMethodCall(node);
			}

			Visit(node.Arguments[0]);

			return node;
		}
	}
}
