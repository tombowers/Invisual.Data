using Invisual.Data.Linq.Helpers;
using Invisual.Data.Linq.QueryHandling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Invisual.Data.Linq
{
	public sealed class DatabaseTable<T> : IOrderedQueryable<T>, IQueryProvider
	{
		private readonly ISqlDataSource _sqlDataSource;

		public DatabaseTable(ISqlDataSource sqlDataSource)
		{
			if (sqlDataSource == null)
				throw new ArgumentNullException("sqlDataSource");

			_sqlDataSource = sqlDataSource;

			Expression = Expression.Constant(this);
		}

		/// <summary>
		/// This constructor is called internally to create a new IQueryable from the specified expression.
		/// </summary>
		/// <param name="expression"></param>
		private DatabaseTable(Expression expression, ISqlDataSource sqlDataSource)
		{
			if (expression == null)
				throw new ArgumentNullException("expression");
			if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
				throw new ArgumentException("expression Type is not compatible with this query provider");
			if (sqlDataSource == null)
				throw new ArgumentNullException("sqlDataSource");

			Expression = expression;
			_sqlDataSource = sqlDataSource;
		}

		#region IOrderedQueryable<T>

		public IEnumerator<T> GetEnumerator()
		{
			return (Provider.Execute<IEnumerable<T>>(Expression)).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (Provider.Execute<IEnumerable>(Expression)).GetEnumerator();
		}

		public Type ElementType
		{
			get { return typeof(T); }
		}

		public Expression Expression { get; private set; }

		public IQueryProvider Provider
		{
			get { return this; }
		}

		#endregion

		#region IQueryProvider

		public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
		{
			return new DatabaseTable<TElement>(expression, _sqlDataSource);
		}

		public IQueryable CreateQuery(Expression expression)
		{
			Type elementType = TypeSystem.GetElementType(expression.Type);
			try
			{
				return (IQueryable)Activator.CreateInstance(typeof(DatabaseTable<>).MakeGenericType(elementType), new object[] { expression, _sqlDataSource });
			}
			catch (TargetInvocationException tie)
			{
				throw tie.InnerException;
			}
		}

		public TResult Execute<TResult>(Expression expression)
		{
			return expression.Execute<TResult>(_sqlDataSource);
		}

		public object Execute(Expression expression)
		{
			return expression.Execute(_sqlDataSource);
		}

		#endregion
	}
}
