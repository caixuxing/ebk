using YueJia.Ebk.Application.Contracts.DeptApp.Commands;
using YueJia.Ebk.Application.Contracts.DeptApp.Dto;
using YueJia.Ebk.Application.Contracts.DeptApp.Query;
using YueJia.Ebk.Domain.Shared.Response;

namespace YueJia.Ebk.Application.Contracts.DeptApp
{
    public interface IDeptApp
    {

        /// <summary>
        /// 创建部门
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        Task<long> CreateDeptAsync(CreateOrUpdateDeptCmd cmd);


        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        Task<bool> DeleteDeptAsync(long id);


        /// <summary>
        /// 更新部门信息
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        Task<bool> UpdateDeptAsync(CreateOrUpdateDeptCmd cmd, long id);


        /// <summary>
        /// 获取部门集合列表
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        Task<PageData<IEnumerable<DeptPageListDto>>> GetPageListDeptAsync(DeptPageListQry qry);


        /// <summary>
        /// 按Id获取部门详情
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        Task<DeptDetailsDto> GetDeptById(long id);


        /// <summary>
        /// 获取顶级部门数据
        /// </summary>
        /// <returns></returns>
        Task<List<SelectDataDto<string>>> GetTopLevelDeptData();



        /// <summary>
        /// 获取部门TreeSelect
        /// </summary>
        /// <returns></returns>
        Task<List<TreeSelectDataDto<string>>> GetDeptTreeSelectData();

    }
}
