using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using SqlSugar.IOC;
using System.Reflection;

namespace YueJia.Ebk.DbMigrator
{
    public static class DependencyInjection
    {
        /// <summary>
        /// 数据库链接
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddInDbMigratorServices(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.GetSection("ConnectionConfigs").Get<IocConfig>()
                ?? throw new InvalidOperationException("请配置数据库链接");
            services.AddSingleton<ISqlSugarClient>(s =>
            {
                SqlSugarScope sqlSugar = new SqlSugarScope(new ConnectionConfig()
                {

                    DbType = (DbType)config.DbType,
                    ConnectionString = config.ConnectionString,
                    IsAutoCloseConnection = config.IsAutoCloseConnection,

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
                   db.Aop.OnLogExecuting = (sql, pars) =>
                   {
                       var Strsql = new KeyValuePair<string, SugarParameter[]>(sql, pars);
                   };
               });
                return sqlSugar;
            });
            services.AddHostedService<DbMigratorHostedService>();
            return services;
        }
    }
}
