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

            //公司与平台 关系
            if (!((await db.Ado.GetIntAsync("SELECT 1 FROM sys.triggers WHERE name = 'add_update_delete_company_channel_map'")) > 0))
            {
                await db.Ado.ExecuteCommandAsync(@"Create trigger [dbo].[add_update_delete_company_channel_map]
on [dbo].[company_channel_map]
    AFTER DELETE,INSERT,UPDATE
as
    begin
		 insert into tracing
		 select id,'company_channel_map',0,GETDATE() from (
				select Id from inserted
				union
				select Id from deleted
			) as T

end");
            }

            //部门与平台 关系
            if (!((await db.Ado.GetIntAsync("SELECT 1 FROM sys.triggers WHERE name = 'add_update_delete_department_channel_map'")) > 0))
            {
                await db.Ado.ExecuteCommandAsync(@"Create  trigger [dbo].[add_update_delete_department_channel_map]
on [dbo].[department_channel_map]
    AFTER DELETE,INSERT,UPDATE
as
    begin
		 -- 新增
		 insert into tracing
		 select id,'department_channel_map',0,GETDATE() from (
				select Id from inserted
				union
				select Id from deleted
			) as T
end");
            }

            //公司
            if (!((await db.Ado.GetIntAsync("SELECT 1 FROM sys.triggers WHERE name = 'add_update_delete_company'")) > 0))
            {
                await db.Ado.ExecuteCommandAsync(@"Create  trigger [dbo].[add_update_delete_company]
    on [dbo].[company]
    AFTER DELETE, INSERT, UPDATE
    as
    begin
        --2 修改
        IF EXISTS (SELECT 1 FROM inserted) AND EXISTS(SELECT 1 FROM Deleted)
            BEGIN
			  	 UPDATE p1
					SET p1.company_status = x1.status,
						p1.adjustment_price_type = x1.adjustment_price_type,
						p1.adjustment_price_value = x1.adjustment_price_value
					FROM inserted x1
				inner join Deleted x2 on x1.id = x2.id and ( x1.status !=x2.status or x1.adjustment_price_type!=x2.adjustment_price_type or x1.adjustment_price_value = x2.adjustment_price_value )
					JOIN hotel_quote p1 ON x1.id = p1.company_id;
            end
        --3  删除
        if NOT EXISTS (SELECT 1 FROM inserted) AND EXISTS(SELECT 1 FROM Deleted)
            BEGIN
				   delete hotel_quote  
				    where  exists(
						select 1 from Deleted p1 where hotel_quote.company_id = p1.id
				   )
            end
    end");
            }

            //用户
            if (!((await db.Ado.GetIntAsync("SELECT 1 FROM sys.triggers WHERE name = 'add_update_delete_sys_user'")) > 0))
            {
                await db.Ado.ExecuteCommandAsync(@"Create  trigger [dbo].[add_update_delete_sys_user]
    on [dbo].[sys_user]
    AFTER DELETE, INSERT, UPDATE
    as
    begin
        --2 修改
        IF EXISTS (SELECT 1 FROM inserted) AND EXISTS(SELECT 1 FROM Deleted)
            BEGIN
			  	 UPDATE p1
					SET p1.sys_user_status = x1.is_enabled,
						p1.dept_id = ISNULL( x1.dept_id,0)
					FROM inserted x1
				inner JOIN Deleted x2 on x1.id = x2.id and (x1.is_enabled !=x2.is_enabled  or x1.dept_id !=x2.dept_id )
				inner JOIN hotel_quote p1 ON  x1.id = p1.user_id 
            end
        --3  删除
        if NOT EXISTS (SELECT 1 FROM inserted) AND EXISTS(SELECT 1 FROM Deleted)
            BEGIN
				   delete hotel_quote  
				    where  exists(
						select 1 from Deleted x1 where x1.id = hotel_quote.user_id
				   )
            end
    end");
            }

            //用户酒店
            if (!((await db.Ado.GetIntAsync("SELECT 1 FROM sys.triggers WHERE name = 'add_update_delete_hotel_publish'")) > 0))
            {
                await db.Ado.ExecuteCommandAsync(@"Create   trigger [dbo].[add_update_delete_hotel_publish]
    on [dbo].[hotel_publish]
    AFTER DELETE, INSERT, UPDATE
    as
    begin
        --2 修改
        IF EXISTS (SELECT 1 FROM inserted) AND EXISTS(SELECT 1 FROM Deleted)
            BEGIN
			  	 UPDATE p1
					SET p1.user_hotel_status = (case when x1.status = 1 then 1 else 0 end   ) 
					FROM inserted x1
				inner JOIN Deleted x2 on x1.id = x2.id and x1.status !=x2.status
				inner JOIN hotel_quote p1 ON  x1.id = p1.user_hotel_id
            end
        --3  删除
        if NOT EXISTS (SELECT 1 FROM inserted) AND EXISTS(SELECT 1 FROM Deleted)
            BEGIN
				   delete hotel_quote  
				    where  exists(
						select 1 from Deleted x1 where x1.id = hotel_quote.user_hotel_id 
				   )
            end
    end");
            }

            //用户房间
            if (!((await db.Ado.GetIntAsync("SELECT 1 FROM sys.triggers WHERE name = 'add_update_delete_hotel_room'")) > 0))
            {
                await db.Ado.ExecuteCommandAsync(@"Create  trigger [dbo].[add_update_delete_hotel_room]
    on [dbo].[hotel_room]
    AFTER DELETE, INSERT, UPDATE
    as
    begin
        --2 修改
        IF EXISTS (SELECT 1 FROM inserted) AND EXISTS(SELECT 1 FROM Deleted)
            BEGIN
			  	 UPDATE p1
					SET p1.user_room_status = x1.is_enabled
					FROM inserted x1
				inner JOIN Deleted x2 on x1.id = x2.id and x1.is_enabled !=x2.is_enabled
				inner JOIN hotel_quote p1 ON  x1.id = p1.user_room_id 
            end
        --3  删除
        if NOT EXISTS (SELECT 1 FROM inserted) AND EXISTS(SELECT 1 FROM Deleted)
            BEGIN
				   delete hotel_quote  
				    where  exists(
						select 1 from Deleted x1 where x1.id = hotel_quote.user_room_id 
				   )
            end
    end");
            }

            //价格计划
            if (!((await db.Ado.GetIntAsync("SELECT 1 FROM sys.triggers WHERE name = 'add_update_delete_price_plan'")) > 0))
            {
                await db.Ado.ExecuteCommandAsync(@"Create  trigger [dbo].[add_update_delete_price_plan]
    on [dbo].[price_plan]
    AFTER DELETE, INSERT, UPDATE
    as
    begin
        --2 修改
        IF EXISTS (SELECT 1 FROM inserted) AND EXISTS(SELECT 1 FROM Deleted)
            BEGIN
			  	 UPDATE p1
					SET p1.user_plan_status = x1.is_enabled
					FROM inserted x1
				inner JOIN Deleted x2 on x1.id = x2.id and x1.is_enabled !=x2.is_enabled
				inner JOIN hotel_quote p1 ON  x1.id = p1.user_price_plan_id 
            end
        --3  删除
        if NOT EXISTS (SELECT 1 FROM inserted) AND EXISTS(SELECT 1 FROM Deleted)
            BEGIN
				   delete hotel_quote  
				    where  exists(
						select 1 from Deleted x1 where x1.id = hotel_quote.user_price_plan_id 
				   )
            end
    end");
            }

            //库存日历
            if (!((await db.Ado.GetIntAsync("SELECT 1 FROM sys.triggers WHERE name = 'add_update_delete_daily_inventory'")) > 0))
            {
                await db.Ado.ExecuteCommandAsync(@"Create  trigger [dbo].[add_update_delete_daily_inventory]
    on [dbo].[daily_inventory]
    AFTER DELETE, INSERT, UPDATE
    as
    begin
        --2 修改
        IF EXISTS (SELECT 1 FROM inserted) AND EXISTS(SELECT 1 FROM Deleted)
            BEGIN
			  	 UPDATE p1
					SET p1.daily_inventory_status = x1.is_enabled,
						p1.inventory_num = x1.inventory_num
					FROM inserted x1
				inner JOIN Deleted x2 on x1.id = x2.id and (x1.is_enabled !=x2.is_enabled or x1.inventory_num !=x2.inventory_num )
				inner JOIN hotel_quote p1 ON  x1.id = p1.daily_inventory_id 
            end
        --3  删除
        if NOT EXISTS (SELECT 1 FROM inserted) AND EXISTS(SELECT 1 FROM Deleted)
            BEGIN
				   delete hotel_quote  
				    where  exists(
						select 1 from Deleted x1 where x1.id = hotel_quote.daily_inventory_id 
				   )
            end
    end");
            }

            //价格日历
            if (!((await db.Ado.GetIntAsync("SELECT 1 FROM sys.triggers WHERE name = 'add_update_delete_daily_price'")) > 0))
            {
                await db.Ado.ExecuteCommandAsync(@" 


ALTER trigger [dbo].[add_update_delete_daily_price]
    on [dbo].[daily_price]
    AFTER DELETE, INSERT, UPDATE
    as
    begin
        --1 新增
        if EXISTS (SELECT 1 FROM inserted) AND NOT EXISTS(SELECT 1 FROM Deleted)
        BEGIN


			 insert into hotel_quote(
user_id,company_id,dept_id,hotel_code,room_code
,user_hotel_id,user_room_id,user_price_plan_id,daily_inventory_id,daily_price_id
,inventory_num,price
,maximum_number_of_people,adult_limit,child_limit,breakfast_type,days_in_advance,continuous_stay_days
,[current_date],last_modified_time
,adjustment_price_type,adjustment_price_value
,company_status,sys_user_status,user_hotel_status,user_room_status,user_plan_status,daily_price_status,daily_inventory_status
)
			 select 
x6.id as user_id,x7.id as company_id, isnull(x6.dept_id,0) as dept_id, x4.hotel_code as hotel_code,x3.room_type as room_code
,x4.id as user_hotel_id,x3.id as user_room_id,x2.id as user_price_plan_id,x5.id as daily_inventory_id,x1.id as daily_price_id
,x5.inventory_num as inventory_num, x1.price as price
,x3.maximum_number_of_people,x3.adult_limit,x3.child_limit,x2.breakfast_type,x2.days_in_advance,x2.continuous_stay_days
,x1.[current_date],GETDATE() as last_modified_time	
,x7.adjustment_price_type,x7. adjustment_price_value	
,x7.status as company_status,
x6.is_enabled as sys_user_status,
(case when x4.status = 1 then 1 else 0 end   ) as user_hotel_status,
 x3.is_enabled user_room_status,
x2.is_enabled as user_plan_status,
x1.is_enabled as daily_price_status,
x5.is_enabled daily_inventory_status
				 
			   from inserted x1     --每日报价 
		inner join price_plan x2 on x1.price_plan_id = x2.id and x1.tenant_id = x2.tenant_id -- 关联价格计划
		inner join hotel_room x3 on x1.room_id = x3.id and x1.tenant_id = x3.tenant_id -- 关联用户房间
		inner join hotel_publish x4 on x3.hotel_id = x4.id and x3.tenant_id = x4.tenant_id -- 关联用户房间
		inner join daily_inventory x5 on x3.id = x5.room_id and x3.tenant_id = x5.tenant_id and x1.[current_date] =x5.[current_date]  -- 关联用户房间
		inner join sys_user x6 on x4.createdby_id = x6.id and x1.tenant_id = x6.tenant_id
		inner join company x7 on  x4.tenant_id = x7.tenant_id
			
		

        end
        --2 修改
        IF EXISTS (SELECT 1 FROM inserted) AND EXISTS(SELECT 1 FROM Deleted)
         BEGIN

				 UPDATE p1
					SET p1.daily_price_status = x1.is_enabled,
						p1.price = x1.price
					FROM inserted x1
				inner JOIN Deleted x2 on x1.id = x2.id and (x1.is_enabled !=x2.is_enabled or x1.price !=x2.price )
				inner JOIN hotel_quote p1 ON  x1.id = p1.daily_price_id 

         end
        --3  删除
        if NOT EXISTS (SELECT 1 FROM inserted) AND EXISTS(SELECT 1 FROM Deleted)
            BEGIN
				   delete hotel_quote  
				    where  exists(
						select 1 from Deleted p1 where hotel_quote.daily_price_id = p1.id 
				   )
				
            end
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
