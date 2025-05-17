using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SqlSugar;
using System.IO.Enumeration;
using System.Reflection;

namespace YueJia.Ebk.DbMigrator
{
    public class DbMigratorHostedService : IHostedService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IConfiguration _configuration;
        readonly IServiceScopeFactory _scopeFactory;

        public DbMigratorHostedService(IHostApplicationLifetime hostApplicationLifetime, IConfiguration configuration, IServiceScopeFactory serviceScope)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _configuration = configuration;
            _scopeFactory = serviceScope;
        }

        /// <summary>
        /// 启动迁移程序
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("正在启动数据迁移！");
            using var serviceScope = _scopeFactory.CreateScope();
            var db = serviceScope.ServiceProvider.GetRequiredService<ISqlSugarClient>().CopyNew();

            Console.WriteLine("正在开始创建数据库！");
            db.DbMaintenance.CreateDatabase();
            Console.WriteLine("数据库创建已完成！");

            Console.WriteLine("正在开始创建表！");
            CreateTable(db);
            Console.WriteLine("表创建已完成！");


            Console.WriteLine("正在开始导入种子数据！");
            await Task.Delay(50);
            Console.WriteLine("种子数据导入已完成！");

            Console.WriteLine("数据迁移已完成！");
        }

        /// <summary>
        ///创建表
        /// </summary>
        /// <param name="db"></param>
        private static void CreateTable(SqlSugarClient db)
        {
            //忽略项
            string directoryPath = AppDomain.CurrentDomain.BaseDirectory;

            var dllList = Directory.GetFiles(directoryPath, "YueJia.Ebk.Domain.dll");

            foreach (string filePath in dllList)
            {
                Assembly assembly = Assembly.LoadFrom(filePath);

                foreach (Type type in assembly.GetTypes())
                {

                    bool isMatch = FileSystemName.MatchesSimpleExpression("YueJia.Ebk.Domain.*.*DO", type.FullName);

                    if (isMatch)
                    {
                        try
                        {
                            db.CodeFirst.InitTables(type);
                            Console.WriteLine($"表 {type.Name} 创建或更新成功！");
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"表 {type.Name} 创建或更新失败：{ex.Message}");
                            Console.ForegroundColor = ConsoleColor.Green;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 结束程序
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"程序结束！！！");
            return Task.CompletedTask;
        }
    }
}
