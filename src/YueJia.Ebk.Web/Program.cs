namespace YueJia.Ebk.Web;

public class Program
{
    public async static Task Main(string[] args)
    {

        try
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.AddAppSettingsSecretsJson().UseAutofac();
            builder.Services.AddControllersWithViews();

            builder.Services.ReplaceConfiguration(builder.Configuration);
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
