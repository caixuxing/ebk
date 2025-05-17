using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net;
using System.Text.Json.Serialization;
using YueJia.Ebk.Domain.Shared.Response;
using YueJia.Ebk.Web.Filter;

namespace YueJia.Ebk.Web;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreMvcModule),
    typeof(YueJiaEbkApplicationContractsModule),
    typeof(YueJiaEbkApplicationModule)
    )]
internal class YueJiaEbkWebModule : AbpModule
{

    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {



        context.Services.AddFluentValidationClientsideAdapters();
        context.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        context.Services.AddControllersWithViews(opt =>
        {
            // 禁用自动模型验证
            opt.ModelValidatorProviders.Clear();
            opt.Filters.Add(typeof(ResultExceptionFilter));
            opt.EnableEndpointRouting = false;
        }).AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);







        //关闭系统自带的模型验证过滤器
        context.Services.Configure<ApiBehaviorOptions>(opt => opt.SuppressModelStateInvalidFilter = true);
        context.Services.AddControllers()
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
        context.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigins",
                builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
        });


        context.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
               .AddCookie(options =>
               {
                   options.LoginPath = "/";
                   options.AccessDeniedPath = "/";
                   options.Cookie.Name = "EbkUser";
                   options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
                   //options.ExpireTimeSpan = TimeSpan.FromSeconds(20);
                   options.SlidingExpiration = true;
                   options.Events = new CookieAuthenticationEvents
                   {
                       OnRedirectToLogin = context =>
                       {
                           context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                           if (context.Request.Headers.ContainsKey("Accept") && (context.Request.Headers["Accept"].FirstOrDefault()?.Contains("application/json") ?? false))
                           {
                               context.Response.ContentType = "application/json";
                               context.Response.StatusCode = 401;
                               context.Response.WriteAsJsonAsync(ServiceResult<string>.Failure("登录已失效，请重新登录"));
                           }
                           else
                           {
                               context.Response.WriteAsync($"<script>top.location.href='{context.RedirectUri}';</script>");
                           }
                           return Task.CompletedTask;
                       }
                   };
               });
        context.Services.AddHttpContextAccessor();




        context.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "后端管理API", Version = "v1" });

            //c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            //{
            //    Description = "在下框中输入请求头中需要添加Jwt授权Token：Bearer Token",
            //    Name = "Authorization",
            //    In = ParameterLocation.Header,
            //    Type = SecuritySchemeType.ApiKey,
            //    BearerFormat = "JWT",
            //    Scheme = "Bearer"
            //});
            ////添加安全要求
            //c.AddSecurityRequirement(new OpenApiSecurityRequirement {
            //    {
            //        new OpenApiSecurityScheme{
            //            Reference =new OpenApiReference{
            //                Type = ReferenceType.SecurityScheme,
            //                Id ="Bearer"
            //            }
            //        },new string[]{ }
            //    }});

            //加载xml文档注释
            Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "CaiXin.*.xml")
             .Select(x => x)
             .ForEach(item => c.IncludeXmlComments(item, true));

            // 开启加权小锁
            //c.OperationFilter<AddResponseHeadersFilter>();
            //c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
        });
        context.Services.AddHttpClient();
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
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "BookStore API");
            options.DefaultModelsExpandDepth(-1); // -1 表示完全隐藏 Schemas 区域
                                                  //options.DefaultModelExpandDepth(0);  // 可选：设置单个模型默认折叠
            options.DocExpansion(DocExpansion.None); // 可选：禁用文档中的默认展开
        });

        return base.OnApplicationInitializationAsync(context);
    }


}