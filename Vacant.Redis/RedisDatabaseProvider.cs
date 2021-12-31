using Newtonsoft.Json;
using StackExchange.Redis;
using StackExchange.Redis.KeyspaceIsolation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Redis
{
    public partial class RedisDatabaseProvider
    {
        private readonly ConnectionMultiplexer _connectionMultiplexer;
        private readonly int _dbNumber;
        private readonly string _keyPrefix;

        /***
         * 文档地址
         * https://www.bookstack.cn/read/StackExchange.Redis-docs-cn/Basics.md
         * https://weihanli.github.io/StackExchange.Redis-docs-zh-cn/
         * https://stackexchange.github.io/StackExchange.Redis/Configuration
         * **/

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="dbNumber">数据库</param>
        /// <param name="keyPrefix">键前缀</param>
        public RedisDatabaseProvider(ConnectionMultiplexer connection, int dbNumber, string keyPrefix)
        {
            _connectionMultiplexer = connection;
            _dbNumber = dbNumber;
            _keyPrefix = keyPrefix;
        }

        public IDatabase Database
        {
            get
            {
                var db = _connectionMultiplexer.GetDatabase(_dbNumber);
                if (_keyPrefix?.Length > 0)
                    return db.WithKeyPrefix(_keyPrefix);
                return db;
            }
        }

        public ITransaction CreateTransaction() => Database.CreateTransaction();
        public IServer GetServer(string hostAndPort)
        {
            return _connectionMultiplexer.GetServer(hostAndPort);
        }

        #region Key
        #region 同步/sync
        public bool Exists(string key, CommandFlags flags = CommandFlags.None)
        {
            return Database.KeyExists(key, flags);
        }

        public long Exists(IEnumerable<string> keys, CommandFlags flags = CommandFlags.None)
        {
            var redisKeys = keys.Select(x => (RedisKey)x).ToArray();
            return Database.KeyExists(redisKeys, flags);
        }

        public bool Remove(string key, CommandFlags flags = CommandFlags.None)
        {
            return Database.KeyDelete(key, flags);
        }

        public long RemoveAll(IEnumerable<string> keys, CommandFlags flags = CommandFlags.None)
        {
            var redisKeys = keys.Select(x => (RedisKey)x).ToArray();
            return Database.KeyDelete(redisKeys, flags);
        }

        #endregion

        #region 异步/Async

        public Task<bool> ExistsAsync(string key, CommandFlags flags = CommandFlags.None)
        {
            return Database.KeyExistsAsync(key, flags);
        }

        /// <inheritdoc/>
        public Task<bool> RemoveAsync(string key, CommandFlags flags = CommandFlags.None)
        {
            return Database.KeyDeleteAsync(key, flags);
        }

        /// <inheritdoc/>
        public Task<long> RemoveAllAsync(IEnumerable<string> keys, CommandFlags flags = CommandFlags.None)
        {
            var redisKeys = keys.Select(x => (RedisKey)x).ToArray();
            return Database.KeyDeleteAsync(redisKeys, flags);
        }


        #endregion
        #endregion



        #region String

        #region 同步/Sync 

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="when"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public bool Add(string key, string value, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            return Database.StringSet(key, value, null, when, flag);
        }
        public bool Add(string key, string value, TimeSpan? expiry = default(TimeSpan?), When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            return Database.StringSet(key, value, expiry, when, flag);
        }
        public bool Add(string key, string value, DateTimeOffset expiresAt, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            var expiration = expiresAt.UtcDateTime.Subtract(DateTime.UtcNow);

            return Database.StringSet(key, value, expiration, when, flag);
        }

        public bool Replace(string key, string value, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            return Add(key, value, When.Always, CommandFlags.None);
        }
        public bool Replace(string key, string value, TimeSpan? expiry = default(TimeSpan?), When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            return Add(key, value, expiry, when, flag);
        }
        public bool Replace(string key, string value, DateTimeOffset expiresAt, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            return Add(key, value, expiresAt, when, flag);
        }






        #endregion

        #region 异步/Async

        #endregion


        #endregion


        private string ConvertToJson<T>(T value)
        {
            string result = value is string ? value.ToString() : JsonConvert.SerializeObject(value);
            return result;
        }

        private T ConvertToObject<T>(RedisValue value)
        {
            if (value.IsNullOrEmpty) return default(T);
            return JsonConvert.DeserializeObject<T>(value);
        }

        private List<T> ConvetToList<T>(RedisValue[] values)
        {
            List<T> result = new List<T>();
            foreach (var item in values)
            {
                var model = ConvertToObject<T>(item);
                result.Add(model);
            }
            return result;
        }
    }
    public partial class RedisDatabaseProvider : IRedisDatabaseProvider
    {
        #region Hash

        #region 同步/Sync 

        /// <summary>
        /// 是否存在hashKey下的子key的数据
        /// </summary>
        /// <param name="hashKey"></param>
        /// <param name="key"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        public bool HashExists(string hashKey, string key, CommandFlags flags = CommandFlags.None)
        {
            return Database.HashExists(hashKey, key, flags);
        }

        public bool HashDelete(string hashKey, string key, CommandFlags flags = CommandFlags.None)
        {

            return Database.HashDelete(hashKey, key, flags);
        }
        public long HashDelete(string hashKey, IEnumerable<string> keys, CommandFlags flags = CommandFlags.None)
        {
            return Database.HashDelete(hashKey, keys.Select(x => (RedisValue)x).ToArray(), flags);
        }


        public bool HashSet<T>(string hashKey, string key, T value, bool nx = false, CommandFlags flags = CommandFlags.None)
        {
            string json = ConvertToJson(value);
            return Database.HashSet(hashKey, key, json, nx ? When.NotExists : When.Always, flags);
        }
        public void HashSet<T>(string hashKey, IDictionary<string, T> values, CommandFlags flags = CommandFlags.None)
        {
            var entries = values.Select(kv => new HashEntry(kv.Key, ConvertToJson(kv.Value)));
            Database.HashSet(hashKey, entries.ToArray(), flags);
        }


        public T HashGet<T>(string hashKey, string key, CommandFlags flags = CommandFlags.None)
        {
            var redisValue = Database.HashGet(hashKey, key, flags);
            //return ConvertObj<T>(redisValue);
            return redisValue.HasValue ? ConvertToObject<T>(redisValue) : default;
        }

        public object HashGet(string hashKey, string key, CommandFlags flags = CommandFlags.None)
        {

            return Database.HashGet(hashKey, key, flags);

        }
        public string HashGetString(string hashKey, string key, CommandFlags flags = CommandFlags.None)
        {
            var redisValue = Database.HashGet(hashKey, key, flags);

            return redisValue.IsNullOrEmpty ? null : redisValue.ToString();
        }

        public Dictionary<string, T> HashGet<T>(string hashKey, IList<string> keys, CommandFlags flags = CommandFlags.None)
        {
            var result = new Dictionary<string, T>();
            for (int i = 0; i < keys.Count; i++)
            {
                result.Add(keys[i], HashGet<T>(hashKey, keys[i], flags));
            }

            return result;
        }

        public Dictionary<string, T> HashGetAll<T>(string hashKey, CommandFlags flags = CommandFlags.None)
        {
            return Database.HashGetAll(hashKey, flags)
                .ToDictionary(
                    x => x.Name.ToString(),
                    x => ConvertToObject<T>(x.Value),
                    StringComparer.Ordinal);
        }

        /// <summary>
        /// 为数字增长value
        /// </summary>
        /// <param name="hashKey"></param>
        /// <param name="key"></param>
        /// <param name="value">可以为负</param>
        /// <returns>增长后的值</returns>
        public double HashIncrement(string hashKey, string key, double value = 1, CommandFlags flags = CommandFlags.None)
        {
            return Database.HashIncrement(key, key, value, flags);
        }

        public long HashIncrement(string hashKey, string key, long value, CommandFlags flags = CommandFlags.None)
        {
            return Database.HashIncrement(key, key, value, flags);
        }

        /// <summary>
        /// 为数字减少value
        /// </summary>
        /// <param name="hashKey"></param>
        /// <param name="key"></param>
        /// <param name="value">可以为负</param>
        /// <returns>减少后的值</returns>
        public double HashDecrement(string hashKey, string key, double value = 1, CommandFlags flags = CommandFlags.None)
        {

            return Database.HashDecrement(key, key, value, flags);
        }
        public long HashDecrement(string hashKey, string key, long value = 1, CommandFlags flags = CommandFlags.None)
        {

            return Database.HashDecrement(key, key, value, flags);
        }


        public long HashCount(string hashKey, CommandFlags flags = CommandFlags.None)
        {
            return Database.HashLength(hashKey, flags);
        }


        public IEnumerable<string> HashKeys(string hashKey, CommandFlags flags = CommandFlags.None)
        {
            return Database.HashKeys(hashKey, flags).Select(x => x.ToString());
            // RedisValue[] values = Database.HashKeys(hashKey, flags);
            //return values.Select(x => x.ToString()).ToList();
        }

        public IEnumerable<T> HashValues<T>(string hashKey, CommandFlags flags = CommandFlags.None)
        {
            return Database.HashValues(hashKey, flags).Select(x => ConvertToObject<T>(x));
        }


        public Dictionary<string, T> HashScan<T>(string hashKey, string pattern, int pageSize = 10, CommandFlags flags = CommandFlags.None)
        {
            return Database.HashScan(hashKey, pattern, pageSize, flags).ToDictionary(x => x.Name.ToString(), x => ConvertToObject<T>(x.Value), StringComparer.Ordinal);
        }
        #endregion

        #region 异步/Async

        /// <inheritdoc/>
        public Task<bool> HashExistsAsync(string hashKey, string key, CommandFlags flags = CommandFlags.None)
        {
            return Database.HashExistsAsync(hashKey, key, flags);
        }

        public Task<bool> HashDeleteAsync(string hashKey, string key, CommandFlags flags = CommandFlags.None)
        {
            return Database.HashDeleteAsync(hashKey, key, flags);
        }

        /// <inheritdoc/>
        public Task<long> HashDeleteAsync(string hashKey, IEnumerable<string> keys, CommandFlags flags = CommandFlags.None)
        {
            return Database.HashDeleteAsync(hashKey, keys.Select(x => (RedisValue)x).ToArray(), flags);
        }


        public Task<bool> HashSetAsync<T>(string hashKey, string key, T value, bool nx = false, CommandFlags flags = CommandFlags.None)
        {
            string json = ConvertToJson(value);
            return Database.HashSetAsync(hashKey, key, json, nx ? When.NotExists : When.Always, flags);
        }

        /// <inheritdoc/>
        public Task HashSetAsync<T>(string hashKey, IDictionary<string, T> values, CommandFlags flags = CommandFlags.None)
        {
            //var entries = values.Select(kv => new HashEntry(kv.Key, Serializer.Serialize(kv.Value)));
            var entries = values.Select(kv => new HashEntry(kv.Key, ConvertToJson(kv.Value)));
            return Database.HashSetAsync(hashKey, entries.ToArray(), flags);
        }





        public async Task<T> HashGetAsync<T>(string hashKey, string key, CommandFlags flags = CommandFlags.None)
        {
            var redisValue = await Database.HashGetAsync(hashKey, key, flags).ConfigureAwait(false);
            //return ConvertObj<T>(redisValue);
            return redisValue.HasValue ? ConvertToObject<T>(redisValue) : default;
        }

        public async Task<object> HashGetAsync(string hashKey, string key, CommandFlags flags = CommandFlags.None)
        {
            var value = await Database.HashGetAsync(hashKey, key, flags);
            return value.IsNullOrEmpty ? null : JsonConvert.DeserializeObject(value);

        }
        public async Task<string> HashGetStringAsync(string hashKey, string key, CommandFlags flags = CommandFlags.None)
        {
            var redisValue = await Database.HashGetAsync(hashKey, key, flags);

            return redisValue.IsNullOrEmpty ? null : redisValue.ToString();
        }

        public async Task<Dictionary<string, T>> HashGetAsync<T>(string hashKey, IList<string> keys, CommandFlags commandFlags = CommandFlags.None)
        {
            var tasks = new Task<T>[keys.Count];

            for (var i = 0; i < keys.Count; i++)
                tasks[i] = HashGetAsync<T>(hashKey, keys[i], commandFlags);

            await Task.WhenAll(tasks).ConfigureAwait(false);

            var result = new Dictionary<string, T>();

            for (var i = 0; i < tasks.Length; i++)
                result.Add(keys[i], tasks[i].Result);

            return result;
        }


        public async Task<Dictionary<string, T>> HashGetAllAsync<T>(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            return (await Database.HashGetAllAsync(hashKey, commandFlags).ConfigureAwait(false))
                .ToDictionary(
                    x => x.Name.ToString(),
                    x => ConvertToObject<T>(x.Value),
                    StringComparer.Ordinal);
        }

        /// <summary>
        /// 为数字增长value
        /// </summary>
        /// <param name="hashKey"></param>
        /// <param name="key"></param>
        /// <param name="value">可以为负</param>
        /// <returns>增长后的值</returns>
        public Task<double> HashIncrementAsync(string hashKey, string key, double value = 1, CommandFlags flags = CommandFlags.None)
        {
            return Database.HashIncrementAsync(key, key, value, flags);
        }

        public Task<long> HashIncrementAsync(string hashKey, string key, long value, CommandFlags flags = CommandFlags.None)
        {
            return Database.HashIncrementAsync(key, key, value, flags);
        }

        /// <summary>
        /// 为数字减少value
        /// </summary>
        /// <param name="hashKey"></param>
        /// <param name="key"></param>
        /// <param name="value">可以为负</param>
        /// <returns>减少后的值</returns>
        public Task<double> HashDecrementAsync(string hashKey, string key, double value = 1, CommandFlags flags = CommandFlags.None)
        {

            return Database.HashDecrementAsync(key, key, value, flags);
        }
        public Task<long> HashDecrementAsync(string hashKey, string key, long value = 1, CommandFlags flags = CommandFlags.None)
        {

            return Database.HashDecrementAsync(key, key, value, flags);
        }


        public Task<long> HashLengthAsync(string hashKey, CommandFlags flags = CommandFlags.None)
        {
            return Database.HashLengthAsync(hashKey, flags);
        }


        public async Task<IEnumerable<string>> HashKeysAsync(string hashKey, CommandFlags flags = CommandFlags.None)
        {
            return (await Database.HashKeysAsync(hashKey, flags).ConfigureAwait(false)).Select(x => x.ToString());
            // RedisValue[] values = Database.HashKeys(hashKey, flags);
            //return values.Select(x => x.ToString()).ToList();
        }

        public async Task<IEnumerable<T>> HashValuesAsync<T>(string hashKey, CommandFlags flags = CommandFlags.None)
        {
            return (await Database.HashValuesAsync(hashKey, flags).ConfigureAwait(false)).Select(x => ConvertToObject<T>(x));
        }

        #endregion

        #endregion
    }

    public partial class RedisDatabaseProvider : IRedisDatabaseProvider
    {
        #region List

        #region 同步方法/Sync

        public long ListAddToLeft<T>(string key, T item, When when = When.Always, CommandFlags flags = CommandFlags.None)
          where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (item == null)
                throw new ArgumentNullException(nameof(item), "item cannot be null.");

            var serializedItem = ConvertToJson(item);

            return Database.ListLeftPush(key, serializedItem, when, flags);
        }

        /// <inheritdoc/>
        public long ListAddToLeft<T>(string key, T[] items, CommandFlags flags = CommandFlags.None)
            where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (items == null)
                throw new ArgumentNullException(nameof(items), "item cannot be null.");

            var serializedItems = items.Select(x => (RedisValue)ConvertToJson(x)).ToArray();

            return Database.ListLeftPush(key, serializedItems, flags);
        }

        /// <inheritdoc/>
        public T ListGetFromRight<T>(string key, CommandFlags flags = CommandFlags.None)
            where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            var item = Database.ListRightPop(key, flags);

            if (item == RedisValue.Null)
                return null;

            return item == RedisValue.Null
                                    ? null
                                    : ConvertToObject<T>(item);
        }

        /// <summary>
        /// 移除指定ListId的内部List的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public long ListRemove<T>(string key, T value)
        {
            return Database.ListRemove(key, ConvertToJson(value));
        }

        /// <summary>
        /// 获取指定key的List
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> ListRange<T>(string key)
        {
            var values = Database.ListRange(key);
            return ConvetToList<T>(values);
        }

        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public long ListRightPush<T>(string key, T value)
        {
            return Database.ListRightPush(key, ConvertToJson(value));
        }

        /// <summary>
        /// 出队
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T ListRightPop<T>(string key)
        {
            var value = Database.ListRightPop(key);
            return ConvertToObject<T>(value);
        }

        /// <summary>
        /// 入栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public long ListLeftPush<T>(string key, T value)
        {
            return Database.ListLeftPush(key, ConvertToJson(value));
        }

        /// <summary>
        /// 出栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T ListLeftPop<T>(string key)
        {
            var value = Database.ListLeftPop(key);
            return ConvertToObject<T>(value);
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long ListLength(string key)
        {
            return Database.ListLength(key);
        }

        #endregion

        #region 异步/Async

        public Task<long> ListAddToLeftAsync<T>(string key, T item, When when = When.Always, CommandFlags flags = CommandFlags.None)
            where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (item == null)
                throw new ArgumentNullException(nameof(item), "item cannot be null.");

            var serializedItem = ConvertToJson(item);

            return Database.ListLeftPushAsync(key, serializedItem, when, flags);
        }

        /// <inheritdoc/>
        public Task<long> ListAddToLeftAsync<T>(string key, T[] items, CommandFlags flags = CommandFlags.None)
            where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (items == null)
                throw new ArgumentNullException(nameof(items), "item cannot be null.");

            var serializedItems = items.Select(x => (RedisValue)ConvertToJson(x)).ToArray();

            return Database.ListLeftPushAsync(key, serializedItems, flags);
        }

        /// <inheritdoc/>
        public async Task<T> ListGetFromRightAsync<T>(string key, CommandFlags flags = CommandFlags.None)
            where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            var item = await Database.ListRightPopAsync(key, flags).ConfigureAwait(false);

            if (item == RedisValue.Null)
                return null;

            return item == RedisValue.Null
                                    ? null
                                    : ConvertToObject<T>(item);
        }



        /// <summary>
        /// 移除指定ListId的内部List的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public Task<long> ListRemoveAsync<T>(string key, T value)
        {
            return Database.ListRemoveAsync(key, ConvertToJson(value));
        }

        /// <summary>
        /// 获取指定key的List
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<T>> ListRangeAsync<T>(string key)
        {
            var values = await Database.ListRangeAsync(key);
            return ConvetToList<T>(values);
        }

        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public Task<long> ListRightPushAsync<T>(string key, T value)
        {
            return Database.ListRightPushAsync(key, ConvertToJson(value));
        }

        /// <summary>
        /// 出队
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> ListRightPopAsync<T>(string key)
        {
            var value = await Database.ListRightPopAsync(key);
            return ConvertToObject<T>(value);
        }

        /// <summary>
        /// 入栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public Task<long> ListLeftPushAsync<T>(string key, T value)
        {
            return Database.ListLeftPushAsync(key, ConvertToJson(value));
        }

        /// <summary>
        /// 出栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> ListLeftPopAsync<T>(string key)
        {
            var value = await Database.ListLeftPopAsync(key);
            return ConvertToObject<T>(value);
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<long> ListLengthAsync(string key)
        {
            return Database.ListLengthAsync(key);
        }



        #endregion

        #endregion
    }


    public partial class RedisDatabaseProvider : IRedisDatabaseProvider
    {
        #region SortedSet 有序集合

        #region 同步/Sync 

        public bool SortedSetAdd<T>(string key, T value, double score, CommandFlags flags = CommandFlags.None)
        {
            string json = ConvertToJson<T>(value);
            return Database.SortedSetAdd(key, json, score, flags);
        }
        public bool SortedSetRemove<T>(string key, T value, CommandFlags flags = CommandFlags.None)
        {
            return Database.SortedSetRemove(key, ConvertToJson(value), flags);
        }

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IEnumerable<T> SortedSetRangeByRank<T>(
                                    string key,
                                    long start = 0,
                                    long stop = -1,
                                    Order order = Order.Ascending,
                                    CommandFlags flags = CommandFlags.None)
        {

            var result = Database.SortedSetRangeByRank(key, start, stop, order, flags);
            //return ConvetToList<T>(result);
            return result.Select(m => m == RedisValue.Null ? default : ConvertToObject<T>(m));
        }

        public IEnumerable<T> SortedSetRangeByScore<T>(
                                   string key,
                                   double start = double.NegativeInfinity,
                                   double stop = double.PositiveInfinity,
                                   Exclude exclude = Exclude.None,
                                   Order order = Order.Ascending,
                                   long skip = 0L,
                                   long take = -1L,
                                   CommandFlags flags = CommandFlags.None)
        {
            var result = Database.SortedSetRangeByScore(key, start, stop, exclude, order, skip, take, flags);

            return result.Select(m => m == RedisValue.Null ? default : ConvertToObject<T>(m));
        }


        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long SortedSetLength(string key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            return Database.SortedSetLength(key, min, max, exclude, flags);
        }


        #endregion

        #region 异步/Async
        public Task<bool> SortedSetAddAsync<T>(string key, T value, double score, CommandFlags flags = CommandFlags.None)
        {
            string json = ConvertToJson<T>(value);
            return Database.SortedSetAddAsync(key, json, score, flags);
        }
        public Task<bool> SortedSetRemoveAsync<T>(string key, T value, CommandFlags flags = CommandFlags.None)
        {
            return Database.SortedSetRemoveAsync(key, ConvertToJson(value), flags);
        }

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> SortedSetRangeByRankAsync<T>(
                                    string key,
                                    long start = 0,
                                    long stop = -1,
                                    Order order = Order.Ascending,
                                    CommandFlags flags = CommandFlags.None)
        {

            var result = await Database.SortedSetRangeByRankAsync(key, start, stop, order, flags).ConfigureAwait(false);
            //return ConvetToList<T>(result);
            return result.Select(m => m == RedisValue.Null ? default : ConvertToObject<T>(m));
        }

        public async Task<IEnumerable<T>> SortedSetRangeByScoreAsync<T>(
                                   string key,
                                   double start = double.NegativeInfinity,
                                   double stop = double.PositiveInfinity,
                                   Exclude exclude = Exclude.None,
                                   Order order = Order.Ascending,
                                   long skip = 0L,
                                   long take = -1L,
                                   CommandFlags flags = CommandFlags.None)
        {
            var result = await Database.SortedSetRangeByScoreAsync(key, start, stop, exclude, order, skip, take, flags).ConfigureAwait(false);

            return result.Select(m => m == RedisValue.Null ? default : ConvertToObject<T>(m));
        }


        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<long> SortedSetLengthAsync(string key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            return Database.SortedSetLengthAsync(key, min, max, exclude, flags);
        }


        #endregion

        #endregion
    }


    public partial class RedisDatabaseProvider : IRedisDatabaseProvider
    {
        #region 发布订阅/PubSub

        #region 同步/Sync 
        /// <summary>
        /// 发布
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public long Publish<T>(RedisChannel channel, T message, CommandFlags flags = CommandFlags.None)
        {
            var sub = _connectionMultiplexer.GetSubscriber();
            return sub.Publish(channel, ConvertToJson(message), flags);
        }

        public void Subscribe<T>(RedisChannel channel, Action<T> handler, CommandFlags flags = CommandFlags.None)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var sub = _connectionMultiplexer.GetSubscriber();

            sub.Subscribe(channel, (redisChannel, value) => handler(ConvertToObject<T>(value)), flags);
        }

        /// <summary>
        /// Redis发布订阅  订阅
        /// </summary>
        /// <param name="subChannel"></param>
        /// <param name="handler"></param>
        public void Subscribe(string subChannel, Action<RedisChannel, RedisValue> handler = null, CommandFlags flags = CommandFlags.None)
        {
            ISubscriber sub = _connectionMultiplexer.GetSubscriber();
            sub.Subscribe(subChannel, (channel, message) =>
            {
                if (handler == null)
                {
                    Console.WriteLine(subChannel + " 订阅收到消息：" + message);
                }
                else
                {
                    handler(channel, message);
                }
            }, flags);
        }
        /// <inheritdoc/>
        public void Unsubscribe<T>(RedisChannel channel, Action<T> handler, CommandFlags flags = CommandFlags.None)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var sub = _connectionMultiplexer.GetSubscriber();
            sub.Unsubscribe(channel, (redisChannel, value) => handler(ConvertToObject<T>(value)), flags);
        }

        /// <summary>
        /// Redis发布订阅  取消订阅
        /// </summary>
        /// <param name="channel"></param>
        public void Unsubscribe(string channel)
        {
            ISubscriber sub = _connectionMultiplexer.GetSubscriber();
            sub.Unsubscribe(channel);
        }

        /// <summary>
        /// Redis发布订阅  取消全部订阅
        /// </summary>
        public void UnsubscribeAll(CommandFlags flags = CommandFlags.None)
        {
            var sub = _connectionMultiplexer.GetSubscriber();
            sub.UnsubscribeAll(flags);
        }


        /// <inheritdoc/>
        public bool UpdateExpiry(string key, DateTimeOffset expiresAt, CommandFlags flags = CommandFlags.None)
        {
            if (Database.KeyExists(key))
                return Database.KeyExpire(key, expiresAt.UtcDateTime.Subtract(DateTime.UtcNow), flags);

            return false;
        }

        /// <inheritdoc/>
        public bool UpdateExpiry(string key, TimeSpan expiresIn, CommandFlags flags = CommandFlags.None)
        {
            if (Database.KeyExists(key))
                return Database.KeyExpire(key, expiresIn, flags);

            return false;
        }

        /// <inheritdoc/>
        public IDictionary<string, bool> UpdateExpiryAll(string[] keys, DateTimeOffset expiresAt, CommandFlags flags = CommandFlags.None)
        {
            var results = new Dictionary<string, bool>(keys.Length, StringComparer.Ordinal);
            for (var i = 0; i < keys.Length; i++)
            {
                results.Add(keys[i], UpdateExpiry(keys[i], expiresAt.UtcDateTime, flags));
            }
            return results;
        }

        /// <inheritdoc/>
        public IDictionary<string, bool> UpdateExpiryAll(string[] keys, TimeSpan expiresIn, CommandFlags flags = CommandFlags.None)
        {
            var results = new Dictionary<string, bool>(keys.Length, StringComparer.Ordinal);
            for (var i = 0; i < keys.Length; i++)
                results.Add(keys[i], UpdateExpiry(keys[i], expiresIn, flags));

            return results;
        }

        #endregion

        #region 异步/Async

        /// <summary>
        /// 发布
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public Task<long> PublishAsync<T>(RedisChannel channel, T message, CommandFlags flags = CommandFlags.None)
        {
            var sub = _connectionMultiplexer.GetSubscriber();
            return sub.PublishAsync(channel, ConvertToJson(message), flags);
        }

        public Task SubscribeAsync<T>(RedisChannel channel, Func<T, Task> handler, CommandFlags flags = CommandFlags.None)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var sub = _connectionMultiplexer.GetSubscriber();
            return sub.SubscribeAsync(channel, async (redisChannel, value) => await handler(ConvertToObject<T>(value)).ConfigureAwait(false), flags);
        }

        /// <summary>
        /// Redis发布订阅  订阅
        /// </summary>
        /// <param name="subChannel"></param>
        /// <param name="handler"></param>
        public Task SubscribeAsync(string subChannel, Action<RedisChannel, RedisValue> handler = null, CommandFlags flags = CommandFlags.None)
        {
            ISubscriber sub = _connectionMultiplexer.GetSubscriber();
            return sub.SubscribeAsync(subChannel, (channel, message) =>
            {
                if (handler == null)
                {
                    Console.WriteLine(subChannel + " 订阅收到消息：" + message);
                }
                else
                {
                    handler(channel, message);
                }
            }, flags);
        }

        public Task UnsubscribeAsync<T>(RedisChannel channel, Func<T, Task> handler, CommandFlags flags = CommandFlags.None)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var sub = _connectionMultiplexer.GetSubscriber();
            return sub.UnsubscribeAsync(channel, (redisChannel, value) => handler(ConvertToObject<T>(value)), flags);
        }
        /// <summary>
        /// Redis发布订阅  取消订阅
        /// </summary>
        /// <param name="channel"></param>
        public Task UnsubscribeAsync(string channel)
        {
            ISubscriber sub = _connectionMultiplexer.GetSubscriber();
            return sub.UnsubscribeAsync(channel);
        }

        /// <summary>
        /// Redis发布订阅  取消全部订阅
        /// </summary>
        public Task UnsubscribeAllAsync(CommandFlags flags = CommandFlags.None)
        {
            var sub = _connectionMultiplexer.GetSubscriber();
            return sub.UnsubscribeAllAsync(flags);
        }


        /// <inheritdoc/>
        public async Task<bool> UpdateExpiryAsync(string key, DateTimeOffset expiresAt, CommandFlags flags = CommandFlags.None)
        {
            if (await Database.KeyExistsAsync(key).ConfigureAwait(false))
                return await Database.KeyExpireAsync(key, expiresAt.UtcDateTime.Subtract(DateTime.UtcNow), flags).ConfigureAwait(false);

            return false;
        }


        public async Task<bool> UpdateExpiryAsync(string key, TimeSpan expiresIn, CommandFlags flags = CommandFlags.None)
        {
            if (await Database.KeyExistsAsync(key).ConfigureAwait(false))
                return await Database.KeyExpireAsync(key, expiresIn, flags).ConfigureAwait(false);

            return false;
        }

        public async Task<IDictionary<string, bool>> UpdateExpiryAllAsync(string[] keys, DateTimeOffset expiresAt, CommandFlags flags = CommandFlags.None)
        {
            var tasks = new Task<bool>[keys.Length];

            for (var i = 0; i < keys.Length; i++)
                tasks[i] = UpdateExpiryAsync(keys[i], expiresAt.UtcDateTime, flags);

            await Task.WhenAll(tasks).ConfigureAwait(false);

            var results = new Dictionary<string, bool>(keys.Length, StringComparer.Ordinal);

            for (var i = 0; i < keys.Length; i++)
                results.Add(keys[i], tasks[i].Result);

            return results;
        }



        public async Task<IDictionary<string, bool>> UpdateExpiryAllAsync(string[] keys, TimeSpan expiresIn, CommandFlags flags = CommandFlags.None)
        {
            var tasks = new Task<bool>[keys.Length];

            for (var i = 0; i < keys.Length; i++)
                tasks[i] = UpdateExpiryAsync(keys[i], expiresIn, flags);

            await Task.WhenAll(tasks).ConfigureAwait(false);

            var results = new Dictionary<string, bool>(keys.Length, StringComparer.Ordinal);

            for (var i = 0; i < keys.Length; i++)
                results.Add(keys[i], tasks[i].Result);

            return results;
        }


        #endregion

        #endregion
    }


}
