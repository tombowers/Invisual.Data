using System;
using System.Linq.Expressions;

namespace Invisual.Data.Linq.QueryHandling.QueryTranslation.SqlSegmentTranslators
{
	public class TakeTranslator : ExpressionVisitor, IQueryTranslator
	{
		public string GetSql()
		{
			return "";
		}

		protected override Expression VisitUnary(UnaryExpression node)
		{
			return base.VisitUnary(node);
		}
	}
}
