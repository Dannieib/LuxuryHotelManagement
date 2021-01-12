using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hotel.Infrastructure.MongoLib
{
    public interface IMongoBaseRepository
    {
        Task<T> InsertHotelRecord<T>(string table, T model);
        Task<IEnumerable<T>> LoadRecord<T>(string tableName, FilterDefinition<T> filter = null, SortDefinition<T> sortDefinition = null);
        Task<T> LoadRecordById<T>(string table, string id);
        Task<(bool isUpdated, string message)> UpdateOrInsert<T>(string table, string id, T record);
        Task<T> LoadRecordByEmailAddress<T>(string table, string emailAddress);
    }
}