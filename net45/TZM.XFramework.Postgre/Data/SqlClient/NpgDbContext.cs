﻿
using Npgsql;

namespace TZM.XFramework.Data.SqlClient
{
    /// <summary>
    /// 数据上下文，表示 Xfw 框架的主入口点
    /// </summary>
    public class NpgDbContext : DbContextBase
    {
        /// <summary>
        /// 查询语义提供者
        /// </summary>
        public override IDbQueryProvider Provider { get { return NpgDbQueryProvider.Instance; } }

        /// <summary>
        /// 初始化 <see cref="NpgDbContext"/> 类的新实例
        /// <para>
        /// 默认读取 XFrameworkConnString 配置里的连接串
        /// </para>
        /// </summary>
        public NpgDbContext()
            : base()
        {
        }

        /// <summary>
        /// 初始化 <see cref="NpgDbContext"/> 类的新实例
        /// <param name="connString">数据库连接字符串</param>
        /// </summary>
        public NpgDbContext(string connString)
            : base(connString)
        {
        }

        /// <summary>
        /// 初始化 <see cref="NpgDbContext"/> 类的新实例
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="commandTimeout">执行命令超时时间</param>
        /// </summary>
        public NpgDbContext(string connString, int? commandTimeout)
            : base(connString, commandTimeout)
        {
        }

        // 创建据库会话实例
        protected override IDatabase CreateDatabase(string connString, int? commandTimeout)
        {
            return new Database(NpgsqlFactory.Instance, connString)
            {
                CommandTimeout = commandTimeout
            };
        }
    }
}
