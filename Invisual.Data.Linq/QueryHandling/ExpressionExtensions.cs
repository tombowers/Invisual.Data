using Invisual.Data.Linq.QueryHandling.QueryTranslation;
using System;
using System.Linq.Expressions;

namespace Invisual.Data.Linq.QueryHandling
{
	public static class ExpressionExtensions
	{
		public static object Execute(this Expression expression, ISqlDataSource dataSource)
		{
			throw new NotImplementedException();
		}

		public static T Execute<T>(this Expression expression, ISqlDataSource dataSource)
		{
			var query = new QueryTranslator<T>(expression).GetSql();

			return default(T);
		}

		public static Expression StripQuotes(this Expression expression)
		{
			while (expression.NodeType == ExpressionType.Quote)
				expression = ((UnaryExpression)expression).Operand;

			return expression;
		}
	}
}
