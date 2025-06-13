using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using SqlSugar;
using System.Diagnostics;
using Volo.Abp.DependencyInjection;
using YueJia.Ebk.Domain.AggRoot;
using YueJia.Ebk.Domain.Hotel;
using YueJia.Ebk.Domain.Other;
using YueJia.Ebk.Domain.Shared.Enums;

namespace YueJia.Ebk.Infrastructure.JobWork
{
    public class EbkDbSyncService : BackgroundService
    {

        public IAbpLazyServiceProvider LazyServiceProvider { get; set; }
        private ISqlSugarClient Db => LazyServiceProvider.LazyGetRequiredService<ISqlSugarClient>();

        private IMongoDatabase MongoDb => LazyServiceProvider.LazyGetRequiredService<IMongoDatabase>();


        private IMongoClient MongoClient => LazyServiceProvider.LazyGetRequiredService<IMongoClient>();

        public EbkDbSyncService(IAbpLazyServiceProvider lazyServiceProvider)
        {
            LazyServiceProvider = lazyServiceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"==================================================数据同步后台服务已启动,当前时间：{DateTime.Now},心跳每2秒执行一次====================================================================");
            var _timer = new PeriodicTimer(TimeSpan.FromSeconds(2));

            while (await _timer.WaitForNextTickAsync(stoppingToken))
            {
                var stopwatch = Stopwatch.StartNew();
                //打印任务执行耗时
                Console.WriteLine($"-----------------------------开始执行数据同步任务！-----------------------------");

                if (await Db.Queryable<TracingDo>().AnyAsync(t => !t.IsDelete && t.State == 0, stoppingToken))
                {

                    var data = await Db.Queryable<TracingDo>().Where(t => !t.IsDelete && t.State == 0).ToListAsync(stoppingToken);
                    var result = data.GroupBy(t => t.TableName).Select(g => new
                    {
                        tableName = g.Key,
                        items = g.Select(m => m.TableId).Distinct().ToList(),
                    }).ToList();


                    foreach (var item in result)
                    {
                        if (item.tableName == "hotel_publish")
                        {
                            var list = await Db.Queryable<HotelPublishDo>().ClearFilter<IDeletedFilter, ITenantIdFilter>().Where(x => item.items.Contains(x.Id)).ToListAsync(stoppingToken);
                            await MongoDb.GetCollection<HotelPublishDo>(nameof(HotelPublishDo)).DeleteManyAsync(x => list.Select(x => x.Id).ToList().Contains(x.Id));


                            var addData = list.Where(x => !x.IsDelete && x.Status == HotelSaleTypeEnum.Up).ToList();
                            if (addData.Count > 0)
                            {
                                await MongoDb.GetCollection<HotelPublishDo>(nameof(HotelPublishDo)).InsertManyAsync(addData, cancellationToken: stoppingToken);
                            }

                        }
                        if (item.tableName == "hotel_room")
                        {
                            var list = await Db.Queryable<HotelRoomDo>().ClearFilter<IDeletedFilter, ITenantIdFilter>().Where(x => item.items.Contains(x.Id)).ToListAsync(stoppingToken);
                            //删除
                            await MongoDb.GetCollection<HotelRoomDo>(nameof(HotelRoomDo)).DeleteManyAsync(x => list.Select(x => x.Id).ToList().Contains(x.Id));
                            //插入

                            var addData = list.Where(x => !x.IsDelete && x.IsEnabled == YesOrNoType.Yes).ToList();
                            if (addData.Count > 0)
                            {
                                await MongoDb.GetCollection<HotelRoomDo>(nameof(HotelRoomDo)).InsertManyAsync(addData, cancellationToken: stoppingToken);
                            }
                        }
                        if (item.tableName == "price_plan")
                        {
                            var list = await Db.Queryable<PricePlanDo>().ClearFilter<IDeletedFilter, ITenantIdFilter>().Where(x => item.items.Contains(x.Id)).ToListAsync(stoppingToken);
                            //删除
                            await MongoDb.GetCollection<PricePlanDo>(nameof(PricePlanDo)).DeleteManyAsync(x => list.Select(x => x.Id).ToList().Contains(x.Id));

                            //插入
                            var addData = list.Where(x => !x.IsDelete && x.IsEnable == YesOrNoType.Yes).ToList();
                            if (addData.Count > 0)
                            {
                                await MongoDb.GetCollection<PricePlanDo>(nameof(PricePlanDo)).InsertManyAsync(addData, cancellationToken: stoppingToken);
                            }
                        }
                        if (item.tableName == "daily_inventory")
                        {
                            var list = await Db.Queryable<DailyInventoryDo>().ClearFilter<IDeletedFilter, ITenantIdFilter>().Where(x => item.items.Contains(x.Id)).ToListAsync(stoppingToken);
                            //删除
                            await MongoDb.GetCollection<DailyInventoryDo>(nameof(DailyInventoryDo)).DeleteManyAsync(x => list.Select(x => x.Id).ToList().Contains(x.Id));
                            //插入
                            var addData = list.Where(x => !x.IsDelete && x.IsEnable == YesOrNoType.Yes).ToList();
                            if (addData.Count > 0)
                            {
                                await MongoDb.GetCollection<DailyInventoryDo>(nameof(DailyInventoryDo)).InsertManyAsync(addData, cancellationToken: stoppingToken);
                            }
                        }
                        if (item.tableName == "daily_price")
                        {
                            var list = await Db.Queryable<DailyPriceDo>().ClearFilter<IDeletedFilter, ITenantIdFilter>().Where(x => item.items.Contains(x.Id)).ToListAsync(stoppingToken);
                            //删除
                            await MongoDb.GetCollection<DailyPriceDo>(nameof(DailyPriceDo)).DeleteManyAsync(x => list.Select(x => x.Id).ToList().Contains(x.Id));
                            //插入

                            var addData = list.Where(x => !x.IsDelete && x.IsEnable == YesOrNoType.Yes).ToList();
                            if (addData.Count > 0)
                            {
                                await MongoDb.GetCollection<DailyPriceDo>(nameof(DailyPriceDo)).InsertManyAsync(addData, cancellationToken: stoppingToken);
                            }
                        }
                    }
                    //回写sqlserver数据同步状态
                    await Db.Fastest<TracingDo>().BulkMergeAsync(data.Select(t => { t.State = 1; return t; }).ToList());

                }
                stopwatch.Stop();
                Console.WriteLine($"-----------------------------数据同步任务执行完毕,总耗时：{stopwatch.ElapsedMilliseconds}毫秒！-----------------------------");
                stopwatch.Restart();

            }
        }
    }
}
