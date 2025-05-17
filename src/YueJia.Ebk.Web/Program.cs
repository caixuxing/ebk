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
            Console.WriteLine($"·þÎñÆô¶¯Ê§°Ü£¡{ex.Message}");

        }
    }

}
