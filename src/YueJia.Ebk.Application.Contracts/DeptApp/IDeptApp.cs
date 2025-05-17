using YueJia.Ebk.Application.Contracts.DeptApp.Dto;
using YueJia.Ebk.Application.Contracts.DeptApp.Query;
using YueJia.Ebk.Domain.Shared.Response;

namespace YueJia.Ebk.Application.Contracts.DeptApp
{
    public interface IDeptApp
    {

        /// <summary>
        /// 获取部门集合列表
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        Task<PageData<IEnumerable<DeptPageListDto>>> GetPageListDeptAsync(DeptPageListQry qry);
    }
}
