using Microsoft.Extensions.Hosting;
using SqlSugar;
using System.Diagnostics;
using Volo.Abp.DependencyInjection;
using YueJia.Ebk.Domain.Other;
using YueJia.Ebk.Domain.Shared.Dto;
using YueJia.Ebk.Domain.Shared.Enums;
using YueJia.Ebk.Domain.Shared.Utils;
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
            var _timer = new PeriodicTimer(TimeSpan.FromSeconds(2));

            while (await _timer.WaitForNextTickAsync(stoppingToken))
            {
                var stopwatch = Stopwatch.StartNew();
                //打印任务执行耗时
                Console.WriteLine($"-----------------------------开始执行发布服务任务！-----------------------------");
                var item = await Db.Queryable<TaskPublishDo>().OrderBy(t => t.CeateTime)
                    .FirstAsync(t => new List<TaskPushStatusTypeEnum>() {
                        TaskPushStatusTypeEnum .Pending,
                        TaskPushStatusTypeEnum.Exception,
                        TaskPushStatusTypeEnum.Failed
                        }.Contains(t.Status) && t.PushCount <= 3, stoppingToken);
                if (item is not null)
                {
                    //解析文本内容
                    var model = System.Text.Json.JsonSerializer.Deserialize<TaskPublishDto>(item.Content);
                    try
                    {
                        item.Status = item.PushType switch
                        {
                            PushTypeEnum.Email => (await EmailService.DoSendAsync(model.RecipientAccount, "订单邮件通知",
                            $"酒店订单通知：{model.OrderCode}，房间：{model.RoomNmae},入住时间：{model.CheckInDate}，离店时间：{model.CheckOutDate}，入住人：{string.Join(",", model.PersonName)}，订单金额：{model.CostAmount}，请您及时确认。") == true ? TaskPushStatusTypeEnum.Success : TaskPushStatusTypeEnum.Failed),
                            PushTypeEnum.SMS => TaskPushStatusTypeEnum.Failed,
                            _ => TaskPushStatusTypeEnum.Failed
                        };
                        item.LastPushTime = DateTime.Now;
                        item.PushCount = item.PushCount + 1;
                        item.ResponseMsg = item.Status.ToDescription();
                    }
                    catch (Exception)
                    {
                        item.Status = TaskPushStatusTypeEnum.Exception;
                        item.LastPushTime = DateTime.Now;
                        item.PushCount = item.PushCount + 1;
                        item.ResponseMsg = item.Status.ToDescription();
                    }
                    await Db.Updateable(item).ExecuteCommandAsync(stoppingToken);
                }
                stopwatch.Stop();
                Console.WriteLine($"-----------------------------发布服务任务执行完毕,总耗时：{stopwatch.ElapsedMilliseconds}毫秒！-----------------------------");
                stopwatch.Restart();

            }
        }
    }


}
