using YueJia.Ebk.Application.Contracts.CompanyApp.Commands;
using YueJia.Ebk.Application.Contracts.CompanyApp.Dto;
using YueJia.Ebk.Application.Contracts.DeptApp;
using YueJia.Ebk.Application.Contracts.DeptApp.Commands;
using YueJia.Ebk.Application.Contracts.DeptApp.Dto;
using YueJia.Ebk.Application.Contracts.DeptApp.Query;
using YueJia.Ebk.Application.Contracts.SysUserApp;
using YueJia.Ebk.Domain.Company;
using YueJia.Ebk.Domain.Dept;
using YueJia.Ebk.Domain.SysUser;

namespace YueJia.Ebk.Application.DeptApp;

[DisableValidation]
public class DeptApp : ApplicationService, IDeptApp
{

    private ISimpleClient<CompanyDO> CompanyRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<CompanyDO>>();
    private ISimpleClient<DepartmentDo> DepartmentRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<DepartmentDo>>();

    private ISqlSugarClient db => LazyServiceProvider.LazyGetRequiredService<ISqlSugarClient>();


    private ICurrentUserApp CurrentUserApp => LazyServiceProvider.LazyGetRequiredService<ICurrentUserApp>();

    public async Task<long> CreateDeptAsync(CreateOrUpdateDeptCmd cmd)
    {
        //验证
        await LazyServiceProvider.LazyGetRequiredService<FluentValidation.IValidator<CreateOrUpdateDeptCmd>>().ValidateAndThrowAsync(cmd);

        if (string.IsNullOrWhiteSpace(CurrentUserApp.Company.CompanyId)) {
            throw new InvalidOperationException("当前用户无公司,无法创建部门！");
        }
        long companyId = long.Parse(CurrentUserApp.Company.CompanyId);

        //校验唯一性
        if (await DepartmentRepo.IsAnyAsync(x => x.Name == cmd.Name && x.CompanyId == companyId)) { 
            throw new InvalidOperationException($"部门【{cmd.Name}】已存在，请更换！");
        }


        var entity = DepartmentDo.Create(name: cmd.Name,
                                    parentId: long.Parse(cmd.ParentDeptId),
                                    companyId: companyId,
                                    status: cmd.Status,
                                    tenantId: CurrentUserApp.TenantId.ToLong()) ?? throw new InvalidOperationException("部门创建失败！");

        return await DepartmentRepo.InsertReturnSnowflakeIdAsync(entity);
    }



    public async Task<bool> DeleteDeptAsync(long id)
    {
        var entity = await DepartmentRepo.GetFirstAsync(x => x.Id == id );
        if(entity == null) {
            throw new InvalidOperationException($"数据未找到！");
        }
        //部门下面存在用户则不允许删除
        if (db.Queryable<SysUserDo>().Any(vv=> vv.DeptId == entity.Id)) { 
            throw new InvalidOperationException($"当前部门存在用户！");
        }

        entity.IsDelete = true;
     
         await DepartmentRepo.AsUpdateable(entity)
         .UpdateColumns(it => new { it.IsDelete, it.Version, it.LastModifiedbyId, it.LastModifiedbyName, it.LastModifiedTime })
         .ExecuteCommandAsync();
        return true;
    }



    public async Task<DeptDetailsDto> GetDeptById(long id)
    {
        var entity = await DepartmentRepo.GetByIdAsync(id) ?? throw new InvalidOperationException($"部门ID:{id}资源不存在！");

        return new DeptDetailsDto()
        {
            Id = entity.Id,
            DeptName = entity.Name,
            CompanyId = entity.CompanyId,
            ParentDeptId = entity.ParentId,
            Status = entity.Status
        };
    }

    public async Task<List<SelectDataDto<string>>> GetTopLevelDeptData()
    {
        var result = await DepartmentRepo.AsQueryable().With(SqlWith.NoLock)
            .Where(x => x.ParentId == -1)
            .Select(x => new SelectDataDto<string>() { Label = x.Name, Value = x.Id.ToString() })
            .ToListAsync();
        result.Insert(0, new SelectDataDto<string>() { Label = "顶级部门", Value = "-1" });
        return result;
    }

    public async Task<PageData<IEnumerable<DeptPageListBaseDto>>> GetPageListDeptAsync(DeptPageListQry qry)
    {
        RefAsync<int> total = 0;

        var query = DepartmentRepo.AsQueryable().With(SqlWith.NoLock)
                                   .LeftJoin<CompanyDO>((t, t1) => t.CompanyId == t1.Id).With(SqlWith.NoLock)
                                   .WhereIF(!string.IsNullOrWhiteSpace(qry.Name), (t, t1) => SqlFunc.Like(t.Name, $"{qry.Name}%"))
                                   .WhereIF(qry.Status.HasValue, (t, t1) => t.Status.Equals(qry.Status))
                                   .Where((t, t1) => t.ParentId == -1)
                                   .Select((t, t1) => new DeptPageListBaseDto()
                                   {
                                       DeptId = t.Id,
                                       DeptName = t.Name,
                                       ParentDeptId = t.ParentId,
                                       CompanyName = t1.Name,
                                       CompanyId = t1.Id,
                                       Status = t.Status,
                                       CreateTime = t.CreateTime,
                                   });
        var data = await query.ToPageListAsync(qry.PageIndex, qry.PageSize, total);


        return new PageData<IEnumerable<DeptPageListBaseDto>>(total, qry.PageSize, qry.PageIndex, data);
    }

    public async Task<bool> UpdateDeptAsync(CreateOrUpdateDeptCmd cmd, long id)
    {
        //验证更新模型参数
        await LazyServiceProvider.LazyGetRequiredService<FluentValidation.IValidator<CreateOrUpdateDeptCmd>>().ValidateAndThrowAsync(cmd);

        //校验唯一性 排除自己
        if (await DepartmentRepo.IsAnyAsync(x => x.Name == cmd.Name && x.Id != id)) throw new InvalidOperationException("部门名称已存在，请更换！");


        //读取公司原始信息
        var entity = await DepartmentRepo.GetByIdAsync(id) ?? throw new InvalidOperationException($"部门ID:{id}资源不存在！");
        //Copy更新信息
        var upEntity = entity with { };
        upEntity.SetName(cmd.Name)
             .SetStatus(cmd.Status);
        //比较更新前后信息
        if (entity.Equals(upEntity)) return true;

        //指定更新列并返回受影响行数
        var affectedRows = await DepartmentRepo.AsUpdateable(upEntity)
           .UpdateColumns(it => new
           {
               it.Name,
               it.Status,
               it.Version,
               it.LastModifiedbyId,
               it.LastModifiedbyName,
               it.LastModifiedTime
           }).ExecuteCommandWithOptLockAsync();
        if (affectedRows is not 1) throw new InvalidOperationException($"部门信息更新失败!");
        return true;
    }

    public async Task<List<TreeSelectDataDto<string>>> GetDeptTreeSelectData()
    {
        var data = await DepartmentRepo.AsQueryable().With(SqlWith.NoLock).ToListAsync();
        return BuildTreeSelect(data);
    }

    private static List<TreeSelectDataDto<string>> BuildTreeSelect(List<DepartmentDo> departments)
    {

        var treeNodes = departments.Select(d => new TreeSelectDataDto<string>
        {
            Value = d.Id.ToString(),
            Label = d.Name
        }).ToList();

        var nodeDict = treeNodes.ToDictionary(n => n.Value);

        foreach (var department in departments)
        {
            if (department.ParentId > 0 && nodeDict.TryGetValue(department.ParentId.ToString(), out var parentNode) && nodeDict.TryGetValue(department.Id.ToString(), out var currentNode))
            {
                parentNode.Children.Add(currentNode);
            }
        }
        return treeNodes.Where(n => departments.FirstOrDefault(d => d.Id.ToString() == n.Value)?.ParentId == -1).ToList();
    }

    public async Task<DepartmentChannelMapModel> GetAssignChannelDetailsAsync(long deptId)
    {
        var deptEntity = await DepartmentRepo.GetByIdAsync(deptId);

        return new DepartmentChannelMapModel
        {
            DeptId = deptEntity.Id,
            DeptName = deptEntity.Name,
            SalePlatCodeList = db.Queryable<DepartmentChannelMapDo>()
                                 .Where(vv => vv.DeptId == deptId)
                                 .Select(vv => vv.SalePlatCode.ToString())
                                 .ToList()
        };
    }


    public async Task<bool> AssignChannelAsync(List<string> salePlatCodeList , long deptId )
    {
        
        var newEntityList = salePlatCodeList.Select(e => DepartmentChannelMapDo.Create(
            deptId,
            e,
            Convert.ToInt64(CurrentUserApp.TenantId))
            ).ToList();

        return await DbTransaction.ExecuteInTransactionAsync(db, async () =>
        {
            //删除
            await db.Deleteable<DepartmentChannelMapDo>().Where(vv => vv.Id == deptId).ExecuteCommandAsync();
            //新增
            await db.Insertable<DepartmentChannelMapDo>(newEntityList).ExecuteReturnSnowflakeIdListAsync();

            return true;
        });
    }


}
