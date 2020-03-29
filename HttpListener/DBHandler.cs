using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Virtualization
{
    public class DBHandler
    {
        private static MongoClient dbClient = new MongoClient("mongodb://127.0.0.1:27017");
        private const string database = "sv";
        private readonly string collection;

        public DBHandler(string collectionName)
        {
            this.collection = collectionName;
        }

        public void CreateNewDocumentInDB(string request, string response)
        {

            IMongoDatabase db = dbClient.GetDatabase(database);

            var req_res = db.GetCollection<BsonDocument>(collection);

            var doc = new BsonDocument
            {
                {"request", request},
                {"response", response}
            };

            req_res.InsertOne(doc);
        }

        public BsonDocument CheckEntryInDB(string request)
        {
            IMongoDatabase db = dbClient.GetDatabase(database);
            var requests = db.GetCollection<BsonDocument>(collection);

            var filter = Builders<BsonDocument>.Filter.Eq("request", request);

            var data = requests.Find(filter).FirstOrDefault();

            return data;
        }
    }
}
