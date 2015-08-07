using System;
using System.Collections.Generic;
using System.Data;

namespace Invisual.Data.Extensions
{
	public static class TypeExtensions
	{
		public static SqlDbType ToSqlDbType(this Type T)
		{
			return SqlTypeMap[T];
		}

		private static readonly Dictionary<Type, SqlDbType> SqlTypeMap = new Dictionary<Type, SqlDbType>
		{
			{ typeof(short), SqlDbType.SmallInt },
			{ typeof(int), SqlDbType.Int },
			{ typeof(long), SqlDbType.BigInt },
			{ typeof(byte[]), SqlDbType.Binary },
			{ typeof(bool), SqlDbType.Bit },
			{ typeof(string), SqlDbType.VarChar },
			{ typeof(DateTime), SqlDbType.Date },
			{ typeof(decimal), SqlDbType.Decimal },
			{ typeof(double), SqlDbType.Float },
			{ typeof(Guid), SqlDbType.UniqueIdentifier }
		};
	}
}
