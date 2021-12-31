using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Redis
{
    public interface IRedisCacheClient
    {
        /// <summary>
        /// Gets an instance of the Redis database for the database 0.
        /// </summary>
        /// <returns>An instance of <see cref="IRedisDatabaseProvider"/>.</returns>
        IRedisDatabaseProvider Db0 { get; }

        /// <summary>
        /// Gets an instance of the Redis database for the database 1.
        /// </summary>
        /// <returns>An instance of <see cref="IRedisDatabaseProvider"/>.</returns>
        IRedisDatabaseProvider Db1 { get; }

        /// <summary>
        /// Gets an instance of the Redis database for the database 2.
        /// </summary>
        /// <returns>An instance of <see cref="IRedisDatabaseProvider"/>.</returns>
        IRedisDatabaseProvider Db2 { get; }

        /// <summary>
        /// Gets an instance of the Redis database for the database  3.
        /// </summary>
        /// <returns>An instance of <see cref="IRedisDatabaseProvider"/>.</returns>
        IRedisDatabaseProvider Db3 { get; }

        /// <summary>
        /// Gets an instance of the Redis database for the database 4.
        /// </summary>
        /// <returns>An instance of <see cref="IRedisDatabaseProvider"/>.</returns>
        IRedisDatabaseProvider Db4 { get; }

        /// <summary>
        /// Gets an instance of the Redis database for the database 5.
        /// </summary>
        /// <returns>An instance of <see cref="IRedisDatabaseProvider"/>.</returns>
        IRedisDatabaseProvider Db5 { get; }

        /// <summary>
        /// Gets an instance of the Redis database for the database 6.
        /// </summary>
        /// <returns>An instance of <see cref="IRedisDatabaseProvider"/>.</returns>
        IRedisDatabaseProvider Db6 { get; }

        /// <summary>
        /// Gets an instance of the Redis database for the database 7.
        /// </summary>
        /// <returns>An instance of <see cref="IRedisDatabaseProvider"/>.</returns>
        IRedisDatabaseProvider Db7 { get; }

        /// <summary>
        /// Gets an instance of the Redis database for the database 8.
        /// </summary>
        /// <returns>An instance of <see cref="IRedisDatabaseProvider"/>.</returns>
        IRedisDatabaseProvider Db8 { get; }

        /// <summary>
        /// Gets an instance of the Redis database for the database 9.
        /// </summary>
        /// <returns>An instance of <see cref="IRedisDatabaseProvider"/>.</returns>
        IRedisDatabaseProvider Db9 { get; }

        /// <summary>
        /// Gets an instance of the Redis database for the database 10.
        /// </summary>
        /// <returns>An instance of <see cref="IRedisDatabaseProvider"/>.</returns>
        IRedisDatabaseProvider Db10 { get; }

        /// <summary>
        /// Gets an instance of the Redis database for the database 11.
        /// </summary>
        /// <returns>An instance of <see cref="IRedisDatabaseProvider"/>.</returns>
        IRedisDatabaseProvider Db11 { get; }

        /// <summary>
        /// Gets an instance of the Redis database for the database 12.
        /// </summary>
        /// <returns>An instance of <see cref="IRedisDatabaseProvider"/>.</returns>
        IRedisDatabaseProvider Db12 { get; }

        /// <summary>
        /// Gets an instance of the Redis database for the database 13.
        /// </summary>
        /// <returns>An instance of <see cref="IRedisDatabaseProvider"/>.</returns>
        IRedisDatabaseProvider Db13 { get; }

        /// <summary>
        /// Gets an instance of the Redis database for the database 14.
        /// </summary>
        /// <returns>An instance of <see cref="IRedisDatabaseProvider"/>.</returns>
        IRedisDatabaseProvider Db14 { get; }

        /// <summary>
        /// Gets an instance of the Redis database for the database 15.
        /// </summary>
        /// <returns>An instance of <see cref="IRedisDatabaseProvider"/>.</returns>
        IRedisDatabaseProvider Db15 { get; }

        /// <summary>
        /// Gets an instance of the Redis database for the database 16.
        /// </summary>
        /// <returns>An instance of <see cref="IRedisDatabaseProvider"/>.</returns>
        IRedisDatabaseProvider Db16 { get; }



        /// <summary>
        /// Returns an instance a Redis databse for the specific database;
        /// </summary>
        /// <param name="dbNumber">The databse number.</param>
        /// <param name="keyPrefix">The key prefix.</param>
        /// <returns>An instance of <see cref="IRedisDatabaseProvider"/>.</returns>
        IRedisDatabaseProvider GetDb(int dbNumber, string keyPrefix = null);

        /// <summary>
        /// Returns an instance a Redis database for the default database present into the configuration file;
        /// </summary>
        /// <returns>An instance of <see cref="IRedisDatabaseProvider"/>.</returns>
        IRedisDatabaseProvider GetDbFromConfiguration();
    }
}
