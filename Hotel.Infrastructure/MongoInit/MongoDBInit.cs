using Hotel.Model;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Infrastructure.MongoInit
{
    public class MongoDBInit : IMongoDBInit
    {
        private readonly AppSettings appSettings;
        private IMongoDatabase db;

        public MongoDBInit(IOptions<AppSettings> options)
        {
            appSettings = options.Value;
        }

        public async Task<IMongoDatabase> InitializeDb()
        {
            try
            {
                var client = new MongoClient(appSettings.MongoDbConn);
                db = client.GetDatabase(appSettings.MongoDatabaseName);

                return await Task.FromResult(db);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
