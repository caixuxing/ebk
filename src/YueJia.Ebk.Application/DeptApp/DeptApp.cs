using YueJia.Ebk.Application.Contracts.DeptApp;
using YueJia.Ebk.Application.Contracts.DeptApp.Dto;
using YueJia.Ebk.Application.Contracts.DeptApp.Query;
using YueJia.Ebk.Domain.Company;
using YueJia.Ebk.Domain.Dept;

namespace YueJia.Ebk.Application.DeptApp
{
    public class DeptApp : ApplicationService, IDeptApp
    {

        private ISimpleClient<CompanyDO> CompanyRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<CompanyDO>>();
        private ISimpleClient<DepartmentDo> DepartmentRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<DepartmentDo>>();

        public async Task<PageData<IEnumerable<DeptPageListDto>>> GetPageListDeptAsync(DeptPageListQry qry)
        {
            RefAsync<int> total = 0;

            var query = DepartmentRepo.AsQueryable()
                   .WhereIF(!string.IsNullOrWhiteSpace(qry.Name), x => SqlFunc.Like(x.Name, $"{qry.Name}%"))
                   .WhereIF(qry.Status.HasValue, x => x.Status.Equals(qry.Status))
                   .Select(x => new DeptPageListDto()
                   {


                   });
            var data = await query.ToPageListAsync(qry.PageIndex, qry.PageSize, total);

            return new PageData<IEnumerable<DeptPageListDto>>(total, qry.PageSize, qry.PageIndex, data);
        }
    }
}
