using YueJia.Ebk.Infrastructure.JobWork;

namespace YueJia.Ebk.Web;

/// <summary>
/// Program
/// </summary>
public class Program
{
    /// <summary>
    /// Main
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public async static Task Main(string[] args)
    {

        try
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.AddAppSettingsSecretsJson().UseAutofac();
            builder.Services.AddControllersWithViews();
            builder.Services.ReplaceConfiguration(builder.Configuration);


            //// 注册自定义序列化器，强制本地时间
            //BsonSerializer.RegisterSerializer(typeof(DateTime),
            //    new DateTimeSerializer(DateTimeKind.Local));
            //BsonSerializer.RegisterSerializer(typeof(DateTime?),
            //    new NullableSerializer<DateTime>(new DateTimeSerializer(DateTimeKind.Local)));

            builder.Services.AddHostedService<EbkDbSyncService>();
            builder.Services.Configure<HostOptions>(options =>
            {
                options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
            });

            await builder.AddApplicationAsync<YueJiaEbkWebModule>();
            var app = builder.Build();
            await app.InitializeApplicationAsync();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Login}/{action=Index}/{id?}");

            await app.RunAsync();

        }
        catch (Exception ex)
        {
            Console.WriteLine($"服务启动失败！{ex.Message}");

        }
    }



    public static Dictionary<string, string> SalePlat = new Dictionary<string, string>() {
      { "A", "携程" },
      { "B", "飞猪" },
      { "D", "去哪" },
      { "F", "途灵" },
      { "G", "美团" },
      { "H", "道旅" },
      { "I", "Etg" },
      { "Z", "Agoda" },
    };

}
