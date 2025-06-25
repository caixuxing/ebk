using Dm;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using SqlSugar;
using System.Collections.Generic;
using System.Diagnostics;
using Volo.Abp.DependencyInjection;
using YueJia.Ebk.Domain.AggRoot;
using YueJia.Ebk.Domain.Company;
using YueJia.Ebk.Domain.Dept;
using YueJia.Ebk.Domain.Hotel;
using YueJia.Ebk.Domain.MongdbModel;
using YueJia.Ebk.Domain.Other;
using YueJia.Ebk.Domain.Shared.Enums;

namespace YueJia.Ebk.Infrastructure.JobWork;

public class EbkDbSyncService : BackgroundService
{

    public IAbpLazyServiceProvider LazyServiceProvider { get; set; }
    private ISqlSugarClient Db => LazyServiceProvider.LazyGetRequiredService<ISqlSugarClient>();

    private IMongoDatabase MongoDb => LazyServiceProvider.LazyGetRequiredService<IMongoDatabase>();


    public EbkDbSyncService(IAbpLazyServiceProvider lazyServiceProvider)
    {
        LazyServiceProvider = lazyServiceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine($"==================================================数据同步后台服务已启动,当前时间：{DateTime.Now},心跳每2秒执行一次====================================================================");
        //var _timer = new PeriodicTimer(TimeSpan.FromSeconds(2));

        //while (await _timer.WaitForNextTickAsync(stoppingToken))
        while(true)
        {
            var stopwatch = Stopwatch.StartNew();
            //打印任务执行耗时
            Console.WriteLine($"-----------------------------开始执行数据同步任务！-----------------------------");

            var tracingList = await Db.Ado.SqlQueryAsync<TracingDo>($@"select top 1 * from tracing where state =0 order by id asc ");
            if (tracingList.Count > 0)
            {

                var tracingObj = tracingList.First();
                if (tracingObj.TableName.ToLower() == "hotel_quote")
                {

                    await MongoDb.GetCollection<HotelQuoteModel>(nameof(HotelQuoteModel)).DeleteManyAsync(x => x.Id == tracingObj.TableId);
                    //同步
                        var dataList = await Db.Queryable<HotelQuoteDo>().Where(x => x.Id == tracingObj.TableId).ToListAsync();
                        if (dataList.Count > 0 &&
                            dataList.First().CompanyStatus &&
                            dataList.First().SysUserStatus &&
                            dataList.First().UserHotelStatus &&
                            dataList.First().UserRoomStatus &&
                            dataList.First().UserPlanStatus &&
                            dataList.First().DailyPriceStatus &&
                            dataList.First().DailyInventoryStatus)
                        {

                            await MongoDb.GetCollection<HotelQuoteModel>(nameof(HotelQuoteModel)).InsertOneAsync(new HotelQuoteModel()
                            {
                                Id = dataList.First().Id,
                                AdultLimit = dataList.First().AdultLimit,
                                BreakfastType = dataList.First().BreakfastType,
                                ChildLimit = dataList.First().ChildLimit,
                                ContinuousStayDays = dataList.First().ContinuousStayDays,
                                CurrentDate = Convert.ToInt32( dataList.First().CurrentDate.ToString("yyyyMMdd")),
                                DaysInAdvance = dataList.First().DaysInAdvance,
                                HotelCode = dataList.First().HotelCode,
                                InventoryNum = dataList.First().InventoryNum,
                                LastModifiedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                MaximumNumberOfPeople = dataList.First().MaximumNumberOfPeople,
                                Price = Convert.ToInt32(dataList.First().Price),
                                RoomCode = dataList.First().RoomCode,
                                UserId = dataList.First().UserId,
                                UserPricePlanId = dataList.First().UserPricePlanId.ToString(),
                                AdjustmentPriceType = dataList.First().AdjustmentPriceType,
                                AdjustmentPriceValue = dataList.First().AdjustmentPriceValue,
                                CompanyId = dataList.First().CompanyId.ToString(),
                                DeptId = dataList.First().DeptId.ToString(),
                            });

                        }
                }
                if (tracingObj.TableName.ToLower() == "department_channel_map")
                { 
                    await MongoDb.GetCollection<CompanyAndDepartmentChannelModel>(nameof(CompanyAndDepartmentChannelModel)).DeleteManyAsync(x => x.TableId == tracingObj.TableId.ToString());
                    //同步
                        var dataList = await Db.Queryable<DepartmentChannelMapDo>().Where(x => x.Id == tracingObj.TableId).ToListAsync();
                        if (dataList.Count > 0 &&
                            dataList.First().IsDelete == false)
                        {
                            await MongoDb.GetCollection<CompanyAndDepartmentChannelModel>(nameof(CompanyAndDepartmentChannelModel)).InsertOneAsync(new CompanyAndDepartmentChannelModel()
                            {
                                  TableId = dataList.First().Id.ToString(),
                                  CompanyAndDepartmentId = dataList.First().DeptId.ToString(),
                                  PFCode = dataList.First().SalePlatCode,
                            });

                        }
                }
                if (tracingObj.TableName.ToLower() == "company_channel_map")
                {
                    await MongoDb.GetCollection<CompanyAndDepartmentChannelModel>(nameof(CompanyAndDepartmentChannelModel)).DeleteManyAsync(x => x.TableId == tracingObj.TableId.ToString());
                    //同步
                    var dataList = await Db.Queryable<CompanyChannelMapDo>().Where(x => x.Id == tracingObj.TableId).ToListAsync();
                    if (dataList.Count > 0 &&
                        dataList.First().IsDelete == false)
                    {
                        await MongoDb.GetCollection<CompanyAndDepartmentChannelModel>(nameof(CompanyAndDepartmentChannelModel)).InsertOneAsync(new CompanyAndDepartmentChannelModel()
                        {
                            TableId = dataList.First().Id.ToString(),
                            CompanyAndDepartmentId = dataList.First().CompanyId.ToString(),
                            PFCode = dataList.First().SalePlatCode,
                        });
                    }
                }

                tracingObj.State = 1;

                await Db.Updateable<TracingDo>(tracingObj).ExecuteCommandAsync();

            }
            else {
                   await Task.Delay(1*1000);
            }
            stopwatch.Stop();
            Console.WriteLine($"-----------------------------数据同步任务执行完毕,总耗时：{stopwatch.ElapsedMilliseconds}毫秒！-----------------------------");
            stopwatch.Restart();

        }
    }
}
