﻿using System;
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
			throw new NotImplementedException();
		}
	}
}