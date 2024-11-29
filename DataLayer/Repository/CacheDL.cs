using DataLayer.Context;
using DataLayer.Interface;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataLayer.Repository
{
    public class CacheDL : ICacheDL
    {
        private readonly IDatabase _cacheDb;
        private readonly IConfiguration _configuration;

        public CacheDL(IConfiguration configuration)
        {
            _configuration = configuration;
            var redis = ConnectionMultiplexer.Connect(_configuration["Redis:ConnectionString"]);
            _cacheDb = redis.GetDatabase();
        }
        public T GetData<T>(string key)
        {
            var value = _cacheDb.StringGet(key);
            if (!string.IsNullOrEmpty(value))
            {
                return JsonSerializer.Deserialize<T>(value);
            }

            return default;
        }

        public object RemoveData(string key)
        {
            var exist = _cacheDb.KeyExists(key);

            if (exist)
            {
                return _cacheDb.KeyDelete(key);
            }
            return false;
        }

        public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var expirtyTime = expirationTime.DateTime.Subtract(DateTime.Now);
            return _cacheDb.StringSet(key, JsonSerializer.Serialize(value), expirtyTime);
        }
    }
}
