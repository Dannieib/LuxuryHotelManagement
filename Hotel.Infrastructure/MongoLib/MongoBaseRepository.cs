using Hotel.Infrastructure.MongoInit;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Infrastructure.MongoLib
{
    public class MongoBaseRepository : IMongoBaseRepository
    {
        private readonly IMongoDBInit _mongoDBInit;
        private IMongoDatabase mongoDatabase;

        public MongoBaseRepository(IMongoDBInit mongoDBInit)
        {
            _mongoDBInit = mongoDBInit;
            mongoDatabase = _mongoDBInit.InitializeDb().Result;
        }

        public async Task<T> InsertHotelRecord<T>(string table, T model)
        {
            try
            {
                var collection = mongoDatabase.GetCollection<T>(table);
                await collection.InsertOneAsync(model);

                return await Task.FromResult(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<(bool isUpdated, string message)> UpdateOrInsert<T>(string table, string id, T record)
        {

            try
            {

                var checkRecord = this.LoadRecordById<T>(table, id);

                if (checkRecord != null)
                {
                    var collection = mongoDatabase.GetCollection<T>(table);
                    var filter = Builders<T>.Filter.Eq("_id", id);
                    ReplaceOneResult updateAction = collection.ReplaceOne(
                        new BsonDocument("_id", id), record, new UpdateOptions { IsUpsert = false });

                    return (true, "Record duly updated");
                }
                return (false, "Record updation was not successful");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<T>> LoadRecord<T>(string tableName, FilterDefinition<T> filter = null, SortDefinition<T> sortDefinition = null)
        {
            try
            {
                if (filter == null)
                    filter = new BsonDocument();

                var collection = mongoDatabase.GetCollection<T>(tableName);

                if (sortDefinition == null)
                    sortDefinition = new BsonDocument();

                var result = collection.Find(filter).Sort(sortDefinition);
                if (result != null)
                    return await result.ToListAsync();

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<T> LoadRecordById<T>(string table, string id)
        {
            var collection = mongoDatabase.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Id", id);

            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<T> LoadRecordByEmailAddress<T>(string table, string emailAddress)
        {
            var collection = mongoDatabase.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("EmailAddress", emailAddress);

            return await collection.Find(filter).FirstOrDefaultAsync();
        }
    }
}
