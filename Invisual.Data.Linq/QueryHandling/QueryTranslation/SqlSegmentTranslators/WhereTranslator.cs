using System;
using System.Linq;
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

		protected override Expression VisitConstant(ConstantExpression node)
		{
			var q = node.Value as IQueryable;
			if (q != null)
			{
				// Assume constant nodes with IQueryables are table references
				// TODO: Refine. Is this needed?
				_sql.Append("SELECT * FROM ");
				_sql.Append(q.ElementType.Name);
			}
			else if (node.Value == null)
			{
				_sql.Append("NULL");
			}
			else
			{
				switch (Type.GetTypeCode(node.Value.GetType()))
				{
					case TypeCode.Boolean:
						_sql.Append(((bool)node.Value) ? 1 : 0);
						break;

					case TypeCode.String:
						_sql.Append("'");
						_sql.Append(node.Value);
						_sql.Append("'");
						break;

					case TypeCode.Object:
						throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", node.Value));

					default:
						_sql.Append(node.Value);
						break;
				}
			}

			return node;
		}

		protected override Expression VisitMember(MemberExpression node)
		{
			if (node.Expression == null || node.Expression.NodeType != ExpressionType.Parameter)
				throw new NotSupportedException(string.Format("The member '{0}' is not supported", node.Member.Name));

			_sql.Append(node.Member.Name);

			return node;
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

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			if (node.Method.DeclaringType != typeof(Queryable) || node.Method.Name != "Where")
				throw new NotSupportedException(string.Format("The method '{0}' is not supported", node.Method.Name));

			_sql.AppendFormat("SELECT * FROM ({0}) AS T WHERE ", Visit(node.Arguments[0]));

			var lambda = (LambdaExpression)StripQuotes(node.Arguments[1]);
			Visit(lambda.Body);

			return node;
		}

		//protected override Expression VisitUnary(UnaryExpression node)
		//{
		//	switch (node.NodeType)
		//	{
		//		case ExpressionType.Not:
		//			_sql.Append(" NOT ");
		//			Visit(node.Operand);
		//			break;

		//		//default:
		//		//	throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", node.NodeType));
		//	}

		//	return node;
		//}

		private static Expression StripQuotes(Expression expression)
		{
			while (expression.NodeType == ExpressionType.Quote)
				expression = ((UnaryExpression)expression).Operand;

			return expression;
		}
	}
}
