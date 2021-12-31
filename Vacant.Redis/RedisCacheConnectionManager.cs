using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Redis
{
    /// <summary>
    /// RedisCache连接管理
    /// </summary>
    public class RedisCacheConnectionManager : IRedisCacheConnectionManager
    {

        private static readonly ILogger<RedisCacheConnectionManager> logger;
        private static readonly object @lock = new object();
        private static ConnectionMultiplexer _connection;
        private static readonly ConcurrentDictionary<string, ConnectionMultiplexer> ConnectionCache = new ConcurrentDictionary<string, ConnectionMultiplexer>();

        public RedisCacheConnectionManager()
        {

        }

        /// <summary>
        /// 键头字符串
        /// </summary>
        public static readonly string KeyPrefix = "HDA_";

        /// <summary>
        /// 单例获取
        /// </summary>
        public static ConnectionMultiplexer Connection
        {
            get
            {
                if (_connection == null)
                {
                    lock (@lock)
                    {
                        if (_connection == null || !_connection.IsConnected)
                        {
                            _connection = GetConnection("");
                        }
                    }
                }
                return _connection;
            }
        }

        /// <summary>
        /// 缓存获取
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static ConnectionMultiplexer GetConnection(string connectionString)
        {
            if (!ConnectionCache.ContainsKey(connectionString))
            {
                var multiplexer = ConnectionMultiplexer.Connect(connectionString);
                ConnectionCache[connectionString] = GetConnectionMultiplexer(multiplexer);
            }
            return ConnectionCache[connectionString];
        }


        //public static ConnectionMultiplexer GetConnection(RedisConfigOptions config)
        //{
        //    string key = config.Url + config.Port;
        //    if (!ConnectionCache.ContainsKey(key))
        //    {
        //        var options = new ConfigurationOptions()
        //        {
        //            SyncTimeout = config.Timeout,
        //            EndPoints =
        //            {
        //                {config.Url,config.Port }
        //            },
        //            Password = config.Password
        //        };
        //        var multiplexer = ConnectionMultiplexer.Connect(options);
        //        ConnectionCache[key] = GetConnectionMultiplexer(multiplexer);
        //    }
        //    return ConnectionCache[key];
        //}

        private static ConnectionMultiplexer GetConnectionMultiplexer(ConnectionMultiplexer multiplexer)
        {
            var connection = multiplexer ?? throw new ArgumentNullException(nameof(multiplexer));
            //注册如下事件
            connection.ConnectionFailed += ConnectionFailed;
            connection.ConnectionRestored += ConnectionRestored;
            connection.ErrorMessage += ErrorMessage;
            connection.ConfigurationChanged += ConfigurationChanged;
            connection.HashSlotMoved += HashSlotMoved;
            connection.InternalError += InternalError;
            return connection;
        }


        #region 事件

        /// <summary>
        /// 配置更改时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ConfigurationChanged(object sender, EndPointEventArgs e)
        {

        }

        /// <summary>
        /// 发生错误时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ErrorMessage(object sender, RedisErrorEventArgs e)
        {
            logger.LogError("Redis error: " + e.Message);
        }

        /// <summary>
        /// 重新建立连接之前的错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            logger.LogError("Redis connection error restored.");
        }

        /// <summary>
        /// 连接失败 ， 如果重新连接成功你将不会收到这个通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            logger.LogError(e.Exception, "Redis connection error {0}.", e.FailureType);
        }

        /// <summary>
        /// 更改集群
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void HashSlotMoved(object sender, HashSlotMovedEventArgs e)
        {

        }

        /// <summary>
        /// redis类库错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void InternalError(object sender, InternalErrorEventArgs e)
        {
            logger.LogError(e.Exception, "Redis internal error {0}.", e.Origin);
        }

        #endregion



    }
}
