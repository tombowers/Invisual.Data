using Invisual.Data.Linq;
using Invisual.Data.Linq.QueryHandling.QueryTranslation;
using Invisual.Data.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester
{
	class Program
	{
		static void Main(string[] args)
		{
			var table = new DatabaseTable<object>(new SqlServerDataSource("test"));

			var test = table
				.Where(x1 => x1 == new object())
				.Where(x2 => x2 == new object())
				.Take(10)
				.ToList();
		}
	}
}
