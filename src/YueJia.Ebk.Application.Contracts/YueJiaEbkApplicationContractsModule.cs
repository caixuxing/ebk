namespace YueJia.Ebk.Application.Contracts;



public class YueJiaEbkApplicationContractsModule : AbpModule
{

    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {

        context.Services.AddFluentValidationClientsideAdapters();
        context.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return base.ConfigureServicesAsync(context);
    }
}
