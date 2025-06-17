using YueJia.Ebk.Api;
try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.AddAppSettingsSecretsJson().UseAutofac();
    builder.Services.AddControllersWithViews();
    builder.Services.ReplaceConfiguration(builder.Configuration);
    await builder.AddApplicationAsync<YueJiaEbkApiModule>();
    var app = builder.Build();
    await app.InitializeApplicationAsync();
    //app.UseAuthorization();

    app.MapControllers();
    await app.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}