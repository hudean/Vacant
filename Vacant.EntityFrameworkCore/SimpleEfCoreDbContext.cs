using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.EntityFrameworkCore
{

    /// <summary>
    /// 简单的数据库上下文
    /// </summary>
    public class SimpleEfCoreDbContext : DbContext
    {
        public SimpleEfCoreDbContext(DbContextOptions<SimpleEfCoreDbContext> options) : base(options)
        {
        }

        /**
        * 打印EFCore生成的sql语句到控制台上  
        * 注意： Program.cs或者appsettings里把Microsoft.EntityFrameworkCore的日志级别设置为warning了要设置为Information
        * **/

        public static readonly ILoggerFactory MyLoggerFactory
        = LoggerFactory.Create(b => b.AddConsole().AddFilter("", LogLevel.Information));
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }
}
