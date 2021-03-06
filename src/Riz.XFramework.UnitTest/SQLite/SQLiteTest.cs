﻿
using System;
using System.Linq;
using System.Collections.Generic;

using Riz.XFramework.Data;
using Riz.XFramework.Data.SqlClient;
using System.Text;
using System.IO;

namespace Riz.XFramework.UnitTest.SQLite
{
    public class SQLiteTest : TestBase<SQLiteModel.Demo>
    {
        //// SQLite 需要将包里的 SQLite.Interop.dll 文件拷到运行目录下

        static string connString = string.Empty;
        public SQLiteTest()
            : base()
        {
            string source = string.Empty;
            var directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (true)
            {
                if (Directory.Exists(Path.Combine(directory.FullName, ".nuget")) &&
                    Directory.Exists(Path.Combine(directory.FullName, "lib")))
                {
                    connString = "DataSource=" + Path.Combine(directory.FullName, "lib") + "\\SQLite\\Riz_XFramework.db;Version=3;Pooling=False;Max Pool Size=100;";
                    break;
                }
                else
                {
                    directory = directory.Parent;
                }
            }
        }

        public override IDbContext CreateDbContext()
        {
            // 直接用无参构造函数时会使用默认配置项 XFrameworkConnString
            var context = new SQLiteDbContext(connString)
            {
                IsDebug = base.IsDebug
            };


            return context;
        }

        protected override void Parameterized()
        {
            var context = _newContext();
            // 构造函数
            var query =
                 from a in context.GetTable<SQLiteModel.Demo>()
                 where a.DemoId <= 10
                 select new SQLiteModel.Demo(a);
            var r1 = query.ToList();
            //SQL=> 
            //SELECT 
            //t0.[DemoId] AS [DemoId],
            //t0.[DemoCode] AS [DemoCode],
            //t0.[DemoName] AS [DemoName],
            //...
            //FROM [Sys_Demo] t0 
            //WHERE t0.[DemoId] <= 10
            query =
               from a in context.GetTable<SQLiteModel.Demo>()
               where a.DemoId <= 10
               select new SQLiteModel.Demo(a.DemoId, a.DemoName);
            r1 = query.ToList();
            //SQL=>
            //SELECT 
            //t0.[DemoId] AS [DemoId],
            //t0.[DemoName] AS [DemoName]
            //FROM [Sys_Demo] t0 
            //WHERE t0.[DemoId] <= 10

        }

        protected override void DbFunction()
        {
            // sqlite 暂时不支持各种函数
            //base.DbFunc();
        }

        protected override void API()
        {
            base.API();

            var context = _newContext();
            DateTime sDate = new DateTime(2007, 6, 10, 0, 0, 0);
            DateTimeOffset sDateOffset = new DateTimeOffset(sDate, new TimeSpan(-7, 0, 0));

            // 批量增加
            // 产生 INSERT INTO VALUES(),(),()... 语法。注意这种批量增加的方法并不能给自增列自动赋值
            var demos = new List<SQLiteModel.Demo>();
            for (int i = 0; i < 5; i++)
            {
                SQLiteModel.Demo d = new SQLiteModel.Demo
                {
                    DemoCode = "D0000001",
                    DemoName = "N0000001",
                    DemoBoolean = true,
                    DemoChar = 'A',
                    DemoNChar = 'B',
                    DemoByte = 64,
                    DemoDate = DateTime.Now,
                    DemoDateTime = DateTime.Now,
                    DemoDateTime2 = DateTime.Now,
                    DemoDecimal = 64,
                    DemoDouble = 64,
                    DemoFloat = 64,
                    DemoGuid = Guid.NewGuid(),
                    DemoShort = 64,
                    DemoInt = 64,
                    DemoLong = 64,
                    DemoTime_Nullable = new TimeSpan(0, 10, 10, 10) + TimeSpan.FromTicks(456789 * 10),
                    DemoDatetimeOffset_Nullable = sDateOffset,
                    DemoTimestamp_Nullable = DateTime.Now,
                    DemoText_Nullable = "TEXT 类型",
                    DemoNText_Nullable = "NTEXT 类型",
                    DemoBinary_Nullable = i % 2 == 0 ? Encoding.UTF8.GetBytes("表示时区偏移量（分钟）（如果为整数）的表达式") : null,
                    DemVarBinary_Nullable = i % 2 == 0 ? Encoding.UTF8.GetBytes(LongText.LONGTEXT) : new byte[0],
                };
                demos.Add(d);
            }
            context.Insert<SQLiteModel.Demo>(demos);
            context.SubmitChanges();
            var myList = context
                .GetTable<SQLiteModel.Demo>()
                .OrderByDescending(x => x.DemoId)
                .Take(5).ToList();

            // byte[]
            var demo = new SQLiteModel.Demo
            {
                DemoCode = "D0000001",
                DemoName = "N0000001",
                DemoBoolean = true,
                DemoChar = 'A',
                DemoNChar = 'B',
                DemoByte = 64,
                DemoDate = DateTime.Now,
                DemoDateTime = DateTime.Now,
                DemoDateTime2 = DateTime.Now,
                DemoDecimal = 64,
                DemoDouble = 64,
                DemoFloat = 64,
                DemoGuid = Guid.NewGuid(),
                DemoShort = 64,
                DemoInt = 64,
                DemoTime_Nullable = new TimeSpan(0, 10, 10, 10) + TimeSpan.FromTicks(456789 * 10),
                DemoDatetimeOffset_Nullable = sDateOffset,//DateTimeOffset.Now,
                DemoTimestamp_Nullable = DateTime.Now,
                DemoText_Nullable = "TEXT 类型",
                DemoNText_Nullable = "NTEXT 类型",
                DemoBinary_Nullable = Encoding.UTF8.GetBytes("表示时区偏移量（分钟）（如果为整数）的表达式"),
                DemVarBinary_Nullable = Encoding.UTF8.GetBytes(LongText.LONGTEXT),
            };
            context.Insert(demo);
            context.SubmitChanges();

            demo = context.GetTable<SQLiteModel.Demo>().FirstOrDefault(x => x.DemoId == demo.DemoId);
        }
    }
}
