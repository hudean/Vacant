using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Redis
{
    public class RedisCacheClient : IRedisCacheClient
    {
        private readonly string keyPrefix;
        private readonly int dbNumber;

        private readonly IConnectionMultiplexer _connectionMultiplexer;
        public RedisCacheClient(
          //RedisCacheConnectionManager connectionManager
          ConnectionMultiplexer redis
          )
        {
            this._connectionMultiplexer = redis;//RedisCacheConnectionManager.GetConnection("");
        }


        /// <inheritdoc/>
        public IRedisDatabaseProvider Db0 => GetDb(0);

        /// <inheritdoc/>
        public IRedisDatabaseProvider Db1 => GetDb(1);

        /// <inheritdoc/>
        public IRedisDatabaseProvider Db2 => GetDb(2);

        /// <inheritdoc/>
        public IRedisDatabaseProvider Db3 => GetDb(3);

        /// <inheritdoc/>
        public IRedisDatabaseProvider Db4 => GetDb(4);

        /// <inheritdoc/>
        public IRedisDatabaseProvider Db5 => GetDb(5);

        /// <inheritdoc/>
        public IRedisDatabaseProvider Db6 => GetDb(6);

        /// <inheritdoc/>
        public IRedisDatabaseProvider Db7 => GetDb(7);

        /// <inheritdoc/>
        public IRedisDatabaseProvider Db8 => GetDb(8);

        /// <inheritdoc/>
        public IRedisDatabaseProvider Db9 => GetDb(9);

        /// <inheritdoc/>
        public IRedisDatabaseProvider Db10 => GetDb(10);

        /// <inheritdoc/>
        public IRedisDatabaseProvider Db11 => GetDb(11);

        /// <inheritdoc/>
        public IRedisDatabaseProvider Db12 => GetDb(12);

        /// <inheritdoc/>
        public IRedisDatabaseProvider Db13 => GetDb(13);

        /// <inheritdoc/>
        public IRedisDatabaseProvider Db14 => GetDb(14);

        /// <inheritdoc/>
        public IRedisDatabaseProvider Db15 => GetDb(15);

        /// <inheritdoc/>
        public IRedisDatabaseProvider Db16 => GetDb(16);

        public IRedisDatabaseProvider GetDb(int dbNumber, string keyPrefix = null)
        {
            //if (string.IsNullOrEmpty(keyPrefix))
            //    keyPrefix = redisConfiguration.KeyPrefix;
            var connection = (ConnectionMultiplexer)_connectionMultiplexer;
            return new RedisDatabaseProvider(connection,
                dbNumber, keyPrefix);
        }

        public IRedisDatabaseProvider GetDbFromConfiguration()
        {
            return GetDb(0, "");
        }
    }
}
