using System;
using System.Linq;
using System.Collections.Generic;

#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using SetUp = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestInitializeAttribute;
using TestFixture = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestClassAttribute;
using Test = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestMethodAttribute;
#else
using NUnit.Framework;
#endif

namespace SQLite.Tests
{
	[TestFixture]
	public class LinqTest
	{
		TestDb CreateDb ()
		{
			var db = new TestDb ();
			db.CreateTable<Product> ();
			db.CreateTable<Order> ();
			db.CreateTable<OrderLine> ();
			db.CreateTable<OrderHistory> ();
			return db;
		}
		
		[Test]
		public void FunctionParameter ()
		{
			var db = CreateDb ();
			
			db.Insert (new Product {
				Name = "A",
				Price = 20,
			});
			
			db.Insert (new Product {
				Name = "B",
				Price = 10,
			});
			
			Func<decimal, List<Product>> GetProductsWithPriceAtLeast = delegate(decimal val) {
				return (from p in db.Table<Product> () where p.Price > val select p).ToList ();
			}; 
			
			var r = GetProductsWithPriceAtLeast (15);
			Assert.AreEqual (1, r.Count);
			Assert.AreEqual ("A", r [0].Name);
		}
		
		[Test]
		public void WhereGreaterThan ()
		{
			var db = CreateDb ();
			
			db.Insert (new Product {
				Name = "A",
				Price = 20,
			});
			
			db.Insert (new Product {
				Name = "B",
				Price = 10,
			});
			
			Assert.AreEqual (2, db.Table<Product> ().Count ());
			
			var r = (from p in db.Table<Product> () where p.Price > 15 select p).ToList ();
			Assert.AreEqual (1, r.Count);
			Assert.AreEqual ("A", r [0].Name);
		}
		
		[Test]
        public void GetWithExpression ()
		{
			var db = CreateDb();
			
			db.Insert (new Product {
				Name = "A",
				Price = 20,
			});
			
			db.Insert (new Product {
				Name = "B",
				Price = 10,
			});

            db.Insert(new Product
            {
                Name = "C",
                Price = 5,
            });
			
			Assert.AreEqual (3, db.Table<Product> ().Count ());
			
			var r = db.Get<Product>(x => x.Price == 10);
            Assert.IsNotNull(r);
			Assert.AreEqual ("B", r.Name);
		}
		
		[Test]
        public void FindWithExpression ()
		{
			var db = CreateDb();
			
			var r = db.Find<Product> (x => x.Price == 10);
            Assert.IsNull (r);
		}
	}
}
