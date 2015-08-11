using Invisual.Data.Linq.QueryHandling.QueryTranslation.SqlSegmentTranslators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Invisual.Data.Linq.QueryHandling.QueryTranslation
{
	public class QueryTranslator<TTable> : ExpressionVisitor, IQueryTranslator
	{
		private readonly List<Expression> _whereClauses;
		private int? _take;

		public QueryTranslator(Expression expression)
		{
			if (expression == null)
				throw new ArgumentNullException("expression");

			_whereClauses = new List<Expression>();

			Visit(expression);
		}

		public string GetSql()
		{
			var sql = new StringBuilder();

			AddSelectStatement(sql);
			AddWhereClauses(sql);
			AddOrderByStatement(sql);

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

				case "Take":
					_take = (int?)((ConstantExpression)node.Arguments[1]).Value;

					break;

				default:
					return base.VisitMethodCall(node);
			}

			Visit(node.Arguments[0]);

			return node;
		}

		private void AddSelectStatement(StringBuilder sql)
		{
			// TODO: Include TOP (Think about MySql LIMIT. Do we need a separate translator to account for syntactical differences?)
			sql.AppendFormat(
				"SELECT{0} {1} FROM {2}",
				_take.HasValue ? string.Format(" TOP {0}", _take) : "",
				"*", // TODO: Select fields based on attributes
				typeof(TTable).Name
				)
				.AppendLine();
		}

		private void AddWhereClauses(StringBuilder sql)
		{
			if (!_whereClauses.Any())
				return;

			var searchConditions = _whereClauses.Select(c => new WhereTranslator(c).GetSql()).ToList();

			sql.AppendLine("WHERE");
			sql.AppendLine(searchConditions[0]);

			for (var i = 1; i < searchConditions.Count(); i++)
				sql.AppendFormat("AND {0}", searchConditions[i]);
		}

		private void AddOrderByStatement(StringBuilder sql)
		{

		}
	}
}
