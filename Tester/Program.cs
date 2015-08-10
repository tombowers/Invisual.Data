using Invisual.Data.Linq;
using Invisual.Data.Linq.QueryHandling.QueryTranslation;
using Invisual.Data.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Tester
{
	class Program
	{
		static void Main(string[] args)
		{
			var db = new TestDatabase();

			var test = db.Table1
				.Where(x => x.Id == "RT354DFGD")
				.Where(x => x.Name == "TestName")
				.OrderBy(x => x.Name)
				.Take(10)
				.ToList();
		}
	}

	public class TestDatabase
	{
		public DatabaseTable<TestTable> Table1
		{
			get
			{
				return new DatabaseTable<TestTable>(new SqlServerDataSource("test"));
			}
		}
	}

	public class TestTable
	{
		public string Id { get; set; }
		public string Name { get; set; }
	}
}
