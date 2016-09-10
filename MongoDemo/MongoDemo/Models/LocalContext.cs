using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MongoDemo.Models
{
    public class LocalContext
    {
        public IMongoDatabase Database;

        public LocalContext()
        {
            MongoClientSettings settings = new MongoClientSettings();
            settings.Server = new MongoServerAddress("localhost", 27017);
            //settings.Server = new MongoServerAddress("192.168.0.103", 27017);
            var client = new MongoClient(settings);
            Database = client.GetDatabase("admin");
        }
        public IMongoCollection<MyClass> MyProp
        {
            get
            {
                return Database.GetCollection<MyClass>("squares");
            }
        }


        public IMongoCollection<Employee> MyEmployee
        {
            get
            {
                return Database.GetCollection<Employee>("Employee");
            }
        }

    }
}