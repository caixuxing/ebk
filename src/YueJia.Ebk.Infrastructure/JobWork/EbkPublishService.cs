using Microsoft.Extensions.Hosting;
using SqlSugar;
using System.Diagnostics;
using Volo.Abp.DependencyInjection;
using YueJia.Ebk.Domain.Other;
using YueJia.Ebk.Infrastructure.Service;

namespace YueJia.Ebk.Infrastructure.JobWork
{

    public class EbkPublishService : BackgroundService
    {

        public IAbpLazyServiceProvider LazyServiceProvider { get; set; } = default!;
        private ISqlSugarClient Db => LazyServiceProvider.LazyGetRequiredService<ISqlSugarClient>();

        private IEmailService EmailService => LazyServiceProvider.LazyGetRequiredService<IEmailService>();



        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            Console.WriteLine($"==================================================数据后台发布服务已启动,当前时间：{DateTime.Now},心跳每5秒执行一次====================================================================");
            var _timer = new PeriodicTimer(TimeSpan.FromSeconds(5));

            while (await _timer.WaitForNextTickAsync(stoppingToken))
            {
                var stopwatch = Stopwatch.StartNew();
                //打印任务执行耗时
                Console.WriteLine($"-----------------------------开始执行发布服务任务！-----------------------------");

                if (await Db.Queryable<TracingDo>().AnyAsync(t => !t.IsDelete, stoppingToken))
                {
                    await EmailService.DoSendAsync();


                    //回写sqlserver数据同步状态
                    // await Db.Fastest<TracingDo>().BulkMergeAsync(data.Select(t => { t.State = 1; return t; }).ToList());

                }
                stopwatch.Stop();
                Console.WriteLine($"-----------------------------发布服务任务执行完毕,总耗时：{stopwatch.ElapsedMilliseconds}毫秒！-----------------------------");
                stopwatch.Restart();

            }
        }
    }


}
