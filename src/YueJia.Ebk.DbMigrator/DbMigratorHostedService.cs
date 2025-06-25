using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SqlSugar;
using System.IO.Enumeration;
using System.Reflection;
using YueJia.Ebk.Domain.Shared.Enums;
using YueJia.Ebk.Domain.SysUser;

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
            await CreateSeedData(db);
            Console.WriteLine("种子数据导入已完成！");


            Console.WriteLine("正在开始创建触发器！");
            await AddTriggers(db);
            Console.WriteLine("触发器创建已完成！");


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




        public async Task CreateSeedData(SqlSugarClient db)
        {
            if (!await db.Queryable<SysUserDo>().AnyAsync(x => x.AccountName == "admin"))
            {
                var entity = SysUserDo.Create("admin", "超级管理员", "admin@ebk.com", AccountTypeEnum.SuperAdmin, YesOrNoType.Yes, null, null);
                entity.TenantId = 1000000000000000000L;
                entity.CreatedbyId = "-1";
                entity.CreatedbyName = "默认用户";
                await db.Insertable(entity).ExecuteReturnSnowflakeIdAsync();
            }
        }

        /// <summary>
        /// 创建触发器
        /// </summary>
        /// <param name="db"></param>
        private async Task AddTriggers(ISqlSugarClient db)
        {
            if (!((await db.Ado.GetIntAsync("SELECT 1 FROM sys.triggers WHERE name = 'add_update_delete_company_channel_map'")) > 0))
            {
                await db.Ado.ExecuteCommandAsync(@"Create trigger [dbo].[add_update_delete_company_channel_map]
                                        on [dbo].[company_channel_map]
                                            AFTER DELETE,INSERT,UPDATE
                                        as
                                            begin

	                                            -- 新增
		                                         insert into tracing
		                                         select id,'company_channel_map',0,GETDATE() from (
				                                        select Id from inserted
				                                        union
				                                        select Id from deleted
			                                        ) as T

                                        end");
            }

            //if (!((await db.Ado.GetIntAsync("SELECT 1 FROM sys.triggers WHERE name = 'add_update_delete_hotel_room'")) > 0))
            //{
            //    await db.Ado.ExecuteCommandAsync(@"Create trigger [dbo].[add_update_delete_hotel_room]
            //on [dbo].[hotel_room]
            //AFTER DELETE, INSERT, UPDATE
            //as
            //begin
            //    --1 新增
            //    if EXISTS (SELECT 1 FROM inserted) AND NOT EXISTS(SELECT 1 FROM Deleted)
            //        BEGIN
            //            insert into tracing
            //            select id, 'hotel_room', 'A', 0, GETDATE(), 0
            //            from inserted
            //        end
            //    --2 修改
            //    IF EXISTS (SELECT 1 FROM inserted) AND EXISTS(SELECT 1 FROM Deleted)
            //        BEGIN
            //            insert into tracing
            //            select id, 'hotel_room', 'U', 0, GETDATE(), 0
            //            from inserted
            //        end
            //    --3  删除
            //    if NOT EXISTS (SELECT 1 FROM inserted) AND EXISTS(SELECT 1 FROM Deleted)
            //        BEGIN
            //            insert into tracing
            //            select id, 'hotel_room', 'D', 0, GETDATE(), 0
            //            from inserted
            //        end
            //end");
            //}

            //if (!((await db.Ado.GetIntAsync("SELECT 1 FROM sys.triggers WHERE name = 'add_update_delete_price_plan'")) > 0))
            //{

            //    await db.Ado.ExecuteCommandAsync(@"CREATE trigger [dbo].[add_update_delete_price_plan]
            //on [dbo].[price_plan]
            //AFTER DELETE, INSERT, UPDATE
            //as
            //begin
            //    --1 新增
            //    if EXISTS (SELECT 1 FROM inserted) AND NOT EXISTS(SELECT 1 FROM Deleted)
            //        BEGIN
            //            insert into tracing
            //            select id, 'price_plan', 'A', 0, GETDATE(), 0
            //            from inserted
            //        end
            //    --2 修改
            //    IF EXISTS (SELECT 1 FROM inserted) AND EXISTS(SELECT 1 FROM Deleted)
            //        BEGIN
            //            insert into tracing
            //            select id, 'price_plan', 'U', 0, GETDATE(), 0
            //            from inserted
            //        end
            //    --3  删除
            //    if NOT EXISTS (SELECT 1 FROM inserted) AND EXISTS(SELECT 1 FROM Deleted)
            //        BEGIN
            //            insert into tracing
            //            select id, 'price_plan', 'D', 0, GETDATE(), 0
            //            from inserted
            //        end
            //end");
            //}

            //if (!((await db.Ado.GetIntAsync("SELECT 1 FROM sys.triggers WHERE name = 'add_update_delete_daily_inventory'")) > 0))
            //{

            //    await db.Ado.ExecuteCommandAsync(@"CREATE trigger [dbo].[add_update_delete_daily_inventory]
            //on [dbo].[daily_inventory]
            //AFTER DELETE, INSERT, UPDATE
            //as
            //begin
            //    --1 新增
            //    if EXISTS (SELECT 1 FROM inserted) AND NOT EXISTS(SELECT 1 FROM Deleted)
            //        BEGIN
            //            insert into tracing
            //            select id, 'daily_inventory', 'A', 0, GETDATE(), 0
            //            from inserted
            //        end
            //    --2 修改
            //    IF EXISTS (SELECT 1 FROM inserted) AND EXISTS(SELECT 1 FROM Deleted)
            //        BEGIN
            //            insert into tracing
            //            select id, 'daily_inventory', 'U', 0, GETDATE(), 0
            //            from inserted
            //        end
            //    --3  删除
            //    if NOT EXISTS (SELECT 1 FROM inserted) AND EXISTS(SELECT 1 FROM Deleted)
            //        BEGIN
            //            insert into tracing
            //            select id, 'daily_inventory', 'D', 0, GETDATE(), 0
            //            from inserted
            //        end
            //end");
            //}

            //if (!((await db.Ado.GetIntAsync("SELECT 1 FROM sys.triggers WHERE name = 'add_update_delete_daily_price'")) > 0))
            //{
            //    await db.Ado.ExecuteCommandAsync(@"CREATE trigger [dbo].[add_update_delete_daily_price]
            //on [dbo].[daily_price]
            //AFTER DELETE, INSERT, UPDATE
            //as
            //begin
            //    --1 新增
            //    if EXISTS (SELECT 1 FROM inserted) AND NOT EXISTS(SELECT 1 FROM Deleted)
            //        BEGIN
            //            insert into tracing
            //            select id, 'daily_price', 'A', 0, GETDATE(), 0
            //            from inserted
            //        end
            //    --2 修改
            //    IF EXISTS (SELECT 1 FROM inserted) AND EXISTS(SELECT 1 FROM Deleted)
            //        BEGIN
            //            insert into tracing
            //            select id, 'daily_price', 'U', 0, GETDATE(), 0
            //            from inserted
            //        end
            //    --3  删除
            //    if NOT EXISTS (SELECT 1 FROM inserted) AND EXISTS(SELECT 1 FROM Deleted)
            //        BEGIN
            //            insert into tracing
            //            select id, 'daily_price', 'D', 0, GETDATE(), 0
            //            from inserted
            //        end
            //end");

            //}
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
