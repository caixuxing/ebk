namespace YueJia.Ebk.Application;


[DependsOn(typeof(YueJiaEbkInfrastructureModule),
           typeof(YueJiaEbkApplicationContractsModule)
)]
public class YueJiaEbkApplicationModule : AbpModule
{


    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {

        return base.ConfigureServicesAsync(context);
    }
}
