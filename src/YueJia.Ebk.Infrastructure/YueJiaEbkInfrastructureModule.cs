using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using SqlSugar.DistributedSystem.Snowflake;
using SqlSugar.IOC;
using System.Reflection;
using YueJia.Ebk.Domain.AggRoot;
using YueJia.Ebk.Domain.Shared.Const;

namespace YueJia.Ebk.Infrastructure;



public class YueJiaEbkInfrastructureModule : AbpModule
{

    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {

        // 注册雪花算法ID生成器为单例
        context.Services.AddSingleton(new IdWorker(1, 1));
        var configuration = context.Services.GetConfiguration();
        var connectionConfigs = configuration.GetSection("ConnectionConfigs").Get<IocConfig>();

        var _accesssor = context.Services.GetRequiredServiceLazy<IHttpContextAccessor>();



        context.Services.AddSingleton<ISqlSugarClient>(s =>
        {
            SqlSugarScope sqlSugar = new SqlSugarScope(new ConnectionConfig()
            {
                DbType = DbType.SqlServer,
                ConnectionString = connectionConfigs?.ConnectionString ?? "",
                IsAutoCloseConnection = true,
                MoreSettings = new ConnMoreSettings()
                {

                    IsAutoRemoveDataCache = true, // 启用自动删除缓存，所有增删改会自动调用.RemoveDataCache()
                    IsAutoDeleteQueryFilter = true, // 启用删除查询过滤器
                    IsAutoUpdateQueryFilter = true, // 启用更新查询过滤器
                    SqlServerCodeFirstNvarchar = true // 采用Nvarchar
                },
                ConfigureExternalServices = new ConfigureExternalServices
                {
                    EntityNameService = (type, entity) => // 处理表
                    {
                        entity.IsDisabledDelete = true; // 禁止删除非 sqlsugar 创建的列
                                                        // 只处理贴了特性[SugarTable]表
                        if (!type.GetCustomAttributes<SugarTable>().Any())
                            return;
                        if (!entity.DbTableName.Contains('_'))
                            entity.DbTableName = UtilMethods.ToUnderLine(entity.DbTableName); // 驼峰转下划线
                    },
                    EntityService = (type, column) => // 处理列
                    {
                        // 只处理贴了特性[SugarColumn]列
                        if (!type.GetCustomAttributes<SugarColumn>().Any())
                            return;
                        if (new NullabilityInfoContext().Create(type).WriteState is NullabilityState.Nullable)
                            column.IsNullable = true;
                        if (!column.IsIgnore && !column.DbColumnName.Contains('_'))
                            column.DbColumnName = UtilMethods.ToUnderLine(column.DbColumnName); // 驼峰转下划线
                    }
                }
            },
           db =>
           {
               // 配置全局过滤器
               db.QueryFilter.AddTableFilter<IDeletedFilter>(it => !it.IsDelete);
               if (!true)
               {
                   db.Aop.OnLogExecuting = (sql, pars) =>
                   {
                       var Strsql = new KeyValuePair<string, SugarParameter[]>(sql, pars);
                   };
                   db.Aop.OnLogExecuting = (sql, pars) =>
                   {
                       //// 若参数值超过100个字符则进行截取
                       //foreach (var par in pars)
                       //{
                       //    if (par.DbType != System.Data.DbType.String || par.Value == null) continue;
                       //    if (par.Value.ToString().Length > 100)
                       //        par.Value = string.Concat(par.Value.ToString()[..100], "......");
                       //}

                       var log = $"【{DateTime.Now}——执行SQL】\r\n{UtilMethods.GetNativeSql(sql, pars)}\r\n";
                       var originColor = Console.ForegroundColor;
                       if (sql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                           Console.ForegroundColor = ConsoleColor.Green;
                       if (sql.StartsWith("UPDATE", StringComparison.OrdinalIgnoreCase) || sql.StartsWith("INSERT", StringComparison.OrdinalIgnoreCase))
                           Console.ForegroundColor = ConsoleColor.Yellow;
                       if (sql.StartsWith("DELETE", StringComparison.OrdinalIgnoreCase))
                           Console.ForegroundColor = ConsoleColor.Red;
                       Console.WriteLine(log);
                       Console.ForegroundColor = originColor;
                       Console.Write(log);
                       //App.PrintToMiniProfiler("SqlSugar", "Info", log);
                   };
                   db.Aop.OnError = ex =>
                   {
                       if (ex.Parametres == null) return;
                       var log = $"【{DateTime.Now}——错误SQL】\r\n{UtilMethods.GetNativeSql(ex.Sql, (SugarParameter[])ex.Parametres)}\r\n";

                       Console.ForegroundColor = ConsoleColor.Red;
                       Console.Write(log);
                       // Log.Error(log, ex);
                       //App.PrintToMiniProfiler("SqlSugar", "Error", log);
                   };
                   db.Aop.OnLogExecuted = (sql, pars) =>
                   {
                       //// 若参数值超过100个字符则进行截取
                       //foreach (var par in pars)
                       //{
                       //    if (par.DbType != System.Data.DbType.String || par.Value == null) continue;
                       //    if (par.Value.ToString().Length > 100)
                       //        par.Value = string.Concat(par.Value.ToString()[..100], "......");
                       //}
                       // 执行时间超过5秒时
                       if (db.Ado.SqlExecutionTime.TotalSeconds > 5)
                       {
                           var fileName = db.Ado.SqlStackTrace.FirstFileName; // 文件名
                           var fileLine = db.Ado.SqlStackTrace.FirstLine; // 行号
                           var firstMethodName = db.Ado.SqlStackTrace.FirstMethodName; // 方法名
                           var log = $"【{DateTime.Now}——超时SQL】\r\n【所在文件名】：{fileName}\r\n【代码行数】：{fileLine}\r\n【方法名】：{firstMethodName}\r\n" + $"【SQL语句】：{UtilMethods.GetNativeSql(sql, pars)}";
                           Console.ForegroundColor = ConsoleColor.Yellow;
                           Console.Write(log);
                           //Log.Warning(log);
                           //App.PrintToMiniProfiler("SqlSugar", "Slow", log);
                       }
                   };
               }
               // 数据审计
               db.Aop.DataExecuting = (oldValue, entityInfo) =>
               {
                   // 新增/插入
                   if (entityInfo.OperationType == DataFilterType.InsertByObject)
                   {
                       // 若主键是长整型且空则赋值雪花Id
                       if (entityInfo.EntityColumnInfo.IsPrimarykey && !entityInfo.EntityColumnInfo.IsIdentity && entityInfo.EntityColumnInfo.PropertyInfo.PropertyType == typeof(long))
                       {
                           var id = entityInfo.EntityColumnInfo.PropertyInfo.GetValue(entityInfo.EntityValue);
                           if (id == null || (long)id == 0)
                               entityInfo.SetValue(0L);
                       }
                       // 若创建时间为空则赋值当前时间
                       else if (entityInfo.PropertyName == nameof(EntityBase.CreateTime) && entityInfo.EntityColumnInfo.PropertyInfo.GetValue(entityInfo.EntityValue) == null)
                       {
                           entityInfo.SetValue(DateTime.Now);
                       }
                       // 若当前用户非空（web线程时）
                       if (_accesssor.Value?.HttpContext?.User != null)
                       {
                           dynamic entityValue = entityInfo.EntityValue;

                           if (entityInfo.PropertyName == nameof(EntityBase.CreatedbyId))
                           {
                               var createUserId = entityValue.CreatedbyId;
                               //if (createUserId == 0 || createUserId == null)
                               if (string.IsNullOrWhiteSpace(createUserId))
                                   entityInfo.SetValue(_accesssor.Value?.HttpContext?.User?.FindFirst(ClaimAttributes.UserId)?.Value);
                           }
                           else if (entityInfo.PropertyName == nameof(EntityBase.CreatedbyName))
                           {
                               var createUserName = entityValue.CreatedbyName;
                               if (string.IsNullOrWhiteSpace(createUserName))
                                   entityInfo.SetValue(_accesssor.Value?.HttpContext?.User?.FindFirst(ClaimAttributes.UserName)?.Value);
                           }
                       }
                   }
                   // 编辑/更新
                   else if (entityInfo.OperationType == DataFilterType.UpdateByObject)
                   {
                       if (entityInfo.PropertyName == nameof(EntityBase.LastModifiedTime))
                           entityInfo.SetValue(DateTime.Now);
                       else if (entityInfo.PropertyName == nameof(EntityBase.LastModifiedbyId))
                       {
                           var _lastModifiedbyId = _accesssor.Value?.HttpContext?.User?.FindFirst(ClaimAttributes.UserId);
                           if (_lastModifiedbyId is not null)
                           {
                               entityInfo.SetValue(_lastModifiedbyId.Value);
                           }
                       }
                       else if (entityInfo.PropertyName == nameof(EntityBase.LastModifiedbyName))
                       {
                           var _lastModifiedbyName = _accesssor.Value?.HttpContext?.User?.FindFirst(ClaimAttributes.UserName);
                           if (_lastModifiedbyName is not null)
                               entityInfo.SetValue(_lastModifiedbyName.Value);
                       }
                   }
               };



           });
            return sqlSugar;
        });
        context.Services.AddScoped(typeof(ISimpleClient<>), typeof(SimpleClient<>)); // 仓储注册


        return base.ConfigureServicesAsync(context);
    }
}
