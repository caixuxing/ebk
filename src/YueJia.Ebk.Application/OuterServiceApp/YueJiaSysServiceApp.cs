using JiebaNet.Segmenter;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using YueJia.Ebk.Application.Contracts.OuterServiceApp;
using YueJia.Ebk.Application.Contracts.OuterServiceApp.Dto;
using YueJia.Ebk.Application.Contracts.OuterServiceApp.Entity;
using YueJia.Ebk.Application.Contracts.OuterServiceApp.Qry;
using YueJia.Ebk.Domain.Shared.Const;

namespace YueJia.Ebk.Application.OuterServiceApp;
public class YueJiaSysServiceApp : ApplicationService, IYueJiaSysServiceApp
{

    private ISqlSugarClient SqlSugarClient => LazyServiceProvider.GetRequiredKeyedService<ISqlSugarClient>(DbConst.YueJiaSysDb);



    public async Task<List<SelectDataDto<int>>> GetDropDownCountryListAsync()
    {

        var data = await SqlSugarClient.Queryable<BAreaEntity>().Where(q => q.level == 1)
           .Select(t => new SelectDataDto<int>()
           {
               Label = $"[{t.CountryIosCode ?? string.Empty}]{t.Name}({t.EnName})",
               Value = t.Id
           }).ToListAsync();

        return data;
    }

    public async Task<PageData<IEnumerable<HotelPageListDto>>> GetHotelPageListAsync(HotelPageListFilterQry qry)
    {

        RefAsync<int> total = 0;


        StringBuilder sql = new StringBuilder($@"SELECT
                        c.name as CountryName,
                        c.enname as CountryNameEn,
                        ba.name as AreaName,
                        ba.enname as AreaNameEn,
                        b.Id,
                        B.hotelcode as HotelCode,
                        b.hotelname as HotelName,
                        b.hotelnameen as HotelNameEn,
                        b.[address] as [Address],
                        b.addressen as AddressEn,
                        b.telphone as TelPhone,
                        b.starlevel as StarLevel
                        FROM b_hotel b
                        LEFT JOIN b_area (nolock) ba ON ba.Id=b.areaid  
					    LEFT JOIN b_area (nolock) c ON c.Id=b.countryid  
                        WHERE   b.status=3  ");
        if (!string.IsNullOrWhiteSpace(qry.HotelName))
        {

            sql.Clear();
            sql.Append($@"SELECT 
                        c.name as CountryName,
                        c.enname as CountryNameEn,
                        ba.name as AreaName,
                        ba.enname as AreaNameEn,
                        b.Id,
                        B.hotelcode as HotelCode,
                        b.hotelname as HotelName,
                        b.hotelnameen as HotelNameEn,
                        b.[address] as [Address],
                        b.addressen as AddressEn,
                        b.telphone as TelPhone,
                        b.starlevel as StarLevel,a.[RANK]
                        FROM FREETEXTTABLE(b_hotel,searchkey,@searchkey,150) a
                        INNER JOIN b_hotel (nolock) b ON a.[KEY]=b.id 
                        LEFT JOIN b_area (nolock) ba ON ba.Id=b.areaid  
						LEFT JOIN b_area (nolock) c ON c.Id=b.countryid  
	                    WHERE b.status=3 ");


        }
        if (!string.IsNullOrWhiteSpace(qry.HotelCode))
        {
            sql.Append(" and hotelcode=@hotelcode");
        }
        if (qry.CountryId.HasValue && qry.CountryId > 0)
        {
            sql.Append(" and countryid=@countryid");
        }

        var query = SqlSugarClient.SqlQueryable<HotelPageListDto>(sql.ToString());




        if (!string.IsNullOrWhiteSpace(qry.HotelName))
        {
            var ParticipleList = Participle(qry.HotelName);
            ParticipleList.Insert(0, qry.HotelName);

            query.OrderBy(" [RANK] DESC ");

            query.AddParameters(new { searchkey = string.Join(" ", ParticipleList.Select(s => $"* {s} *")) });

        }

        if (!string.IsNullOrWhiteSpace(qry.HotelCode))
        {
            query.AddParameters(new { hotelcode = qry.HotelCode });
        }
        if (qry.CountryId.HasValue && qry.CountryId > 0)
        {
            query.AddParameters(new { countryid = qry.CountryId });
        }


        var data = await query.ToPageListAsync(qry.PageIndex, qry.PageSize, total);
        return new PageData<IEnumerable<HotelPageListDto>>(total, qry.PageSize, qry.PageIndex, data);

    }


    /// <summary>
    /// 字符串分词
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private static List<string> Participle(string str) => new JiebaSegmenter().Cut(str, cutAll: true)
                                                                                 .Select(x => x.ToLower())
                                                                                 .Where(x => !string.IsNullOrWhiteSpace(x))
                                                                                 .ToList();


}
