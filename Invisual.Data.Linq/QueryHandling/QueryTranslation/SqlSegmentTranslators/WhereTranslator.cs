using System;
using System.Linq.Expressions;
using System.Text;

namespace Invisual.Data.Linq.QueryHandling.QueryTranslation.SqlSegmentTranslators
{
	public class WhereTranslator : ExpressionVisitor, IQueryTranslator
	{
		private readonly StringBuilder _sql;

		public WhereTranslator(Expression expression)
		{
			if (expression == null)
				throw new ArgumentNullException("expression");

			_sql = new StringBuilder();

			Visit(expression);
		}

		public string GetSql()
		{
			return _sql.ToString();
		}

		protected override Expression VisitBinary(BinaryExpression node)
		{
			_sql.Append("(");
			
			Visit(node.Left);

			switch (node.NodeType)
			{
				case ExpressionType.And:
					_sql.Append(" AND ");
					break;

				case ExpressionType.Or:
					_sql.Append(" OR");
					break;

				case ExpressionType.Equal:
					_sql.Append(" = ");
					break;

				case ExpressionType.NotEqual:
					_sql.Append(" <> ");
					break;

				case ExpressionType.LessThan:
					_sql.Append(" < ");
					break;

				case ExpressionType.LessThanOrEqual:
					_sql.Append(" <= ");
					break;

				case ExpressionType.GreaterThan:
					_sql.Append(" > ");
					break;

				case ExpressionType.GreaterThanOrEqual:
					_sql.Append(" >= ");
					break;

				default:
					throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", node.NodeType));
			}

			Visit(node.Right);

			_sql.Append(")");

			return node;
		}
	}
}
