using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using NUglify.Helpers;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Volo.Abp;
using Volo.Abp.AspNetCore;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Auditing;
using Volo.Abp.Autofac;
using Volo.Abp.Json;
using Volo.Abp.Modularity;
using YueJia.Ebk.Api.Filter;
using YueJia.Ebk.Application;
using YueJia.Ebk.Application.Contracts;

namespace YueJia.Ebk.Api
{

    [DependsOn(
    typeof(AbpAutofacModule),
   typeof(AbpAspNetCoreMvcModule),
        typeof(AbpAspNetCoreModule),
    typeof(YueJiaEbkApplicationContractsModule),
    typeof(YueJiaEbkApplicationModule)
    )]
    internal class YueJiaEbkApiModule : AbpModule
    {

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {



            context.Services.AddFluentValidationClientsideAdapters();
            context.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            //关闭系统自带的模型验证过滤器
            context.Services.Configure<ApiBehaviorOptions>(opt => opt.SuppressModelStateInvalidFilter = true);
            context.Services.AddControllers(opt =>
                {
                    // 禁用自动模型验证
                    opt.ModelValidatorProviders.Clear();
                    opt.Filters.Add(typeof(ResultExceptionFilter));
                    opt.EnableEndpointRouting = false;
                })
                .AddJsonOptions(options =>
                {

                    //数据格式首字母小写
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    //数据格式原样输出
                    // options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    //取消Unicode编码
                    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                    //忽略空值
                    // options.JsonSerializerOptions.IgnoreNullValues = true;
                    //允许额外符号
                    options.JsonSerializerOptions.AllowTrailingCommas = true;
                    //反序列化过程中属性名称是否使用不区分大小写的比较
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
                    //options.JsonSerializerOptions.Converters.Add(new DecimalPrecisionConverter());
                    //options.JsonSerializerOptions.Converters.Add(new ObjectIdJsonConverter());
                    options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.WriteAsString;
                });
            //时间格式化
            Configure<AbpJsonOptions>(options => options.OutputDateTimeFormat = "yyyy-MM-dd HH:mm:ss");
            context.Services.AddEndpointsApiExplorer();
            context.Services.AddHttpContextAccessor();
            context.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ebk服务 API", Version = "v1" });
                //加载xml文档注释
                Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "YueJia.Ebk.*.xml")
                 .Select(x => x)
                 .ForEach(item => c.IncludeXmlComments(item, true));
            });
            context.Services.AddHttpClient();
            Configure<AbpAuditingOptions>(options =>
            {
                // 启用审计日志
                options.IsEnabled = true;
                // 隐藏敏感数据
                options.HideErrors = false;
                //启用记录get请求
                options.IsEnabledForGetRequests = true;

                // 自定义应用名称
                options.ApplicationName = "MyApplication";
            });

            Configure<AbpAuditingOptions>(options =>
            {
                options.EntityHistorySelectors.AddAllEntities();
            });

            return base.ConfigureServicesAsync(context);
        }

        public override Task OnPreApplicationInitializationAsync(ApplicationInitializationContext context)
        {
            return base.OnPreApplicationInitializationAsync(context);
        }
        public override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "YueEbk API");
                options.DefaultModelsExpandDepth(-1); // -1 表示完全隐藏 Schemas 区域
                                                      //options.DefaultModelExpandDepth(0);  // 可选：设置单个模型默认折叠
                options.DocExpansion(DocExpansion.None); // 可选：禁用文档中的默认展开
            });
            app.UseAuditing();

            return base.OnApplicationInitializationAsync(context);
        }
    }
}
