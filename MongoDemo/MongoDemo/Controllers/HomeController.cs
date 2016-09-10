using MongoDB.Bson;
using MongoDB.Driver;
using MongoDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MongoDemo.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            try
            {
                //var data2 = MyProp.Find(Builders<MyClass>.Filter.Gt(x=>x.));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View();
        }

        public ActionResult Create()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [HttpPost]
        public ActionResult Create(Employee emp)
        {
            ViewBag.Message = "Your application description page.";
            LocalContext context = new LocalContext();
            
            context.MyEmployee.InsertOne(emp);

            return View();
        }

        public ActionResult Edit()
        {
            ViewBag.Message = "Your application description page.";
            LocalContext context = new LocalContext();
            var data1 = context.MyEmployee.Find(Builders<Employee>.Filter
                .Where(x => x.n == 2)).FirstOrDefault();
            data1.square = 1234;
            var data2 = context.MyEmployee.Find(x => x.n == 99).FirstOrDefault();
            var data3 = context.MyEmployee.Find(x => x.n == 2);
            var data4 = context.MyEmployee.Find(new BsonDocument {
                {"n", 2}
            }).FirstOrDefault();
            var data5 = context.MyEmployee.Find(Builders<Employee>.Filter.Gte(x=>x.n,44)).ToList();
            var data6 = context.MyEmployee.Find(Builders<Employee>.Filter.In(x => 
                x.n, new List<int>() { 88,99 }
            )).ToList();
            var filter1 = Builders<Employee>.Filter.Empty;
            filter1 = filter1 & Builders<Employee>.Filter.Eq(x=>x.n,44);
            filter1 = filter1 & Builders<Employee>.Filter.Eq(x => x.n, 88);
            var filter2 = context.MyEmployee.Find(filter1).ToList();

            var data7 = context.MyEmployee.Find(Builders<Employee>.Filter.Gte(x => x.n, 44))
                        .Sort(Builders<Employee>.Sort.Descending(x => x.n)).ToList();
            var data8 = context.MyEmployee.Find(Builders<Employee>.Filter.Gte(x => x.n, 44))
                        .SortBy(x=>x.n);

            //To replace the entire document except for the _id field, pass an entirely new document 
            //as the second argument.
            context.MyEmployee.ReplaceOne<Employee>(x => x.n == 2, data1);
            //context.MyEmployee.DeleteOne<Employee>(x => x.n == 44);
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}