using System;

namespace Invisual.Data.Linq.QueryHandling.QueryTranslation
{
	interface IQueryTranslator
	{
		string GetSql();
	}
}
