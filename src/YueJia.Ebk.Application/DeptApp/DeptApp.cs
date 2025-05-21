using YueJia.Ebk.Application.Contracts.Comm.BaseObj;
using YueJia.Ebk.Application.Contracts.DeptApp;
using YueJia.Ebk.Application.Contracts.DeptApp.Commands;
using YueJia.Ebk.Application.Contracts.DeptApp.Dto;
using YueJia.Ebk.Application.Contracts.DeptApp.Query;
using YueJia.Ebk.Application.Contracts.UserApp;
using YueJia.Ebk.Domain.Company;
using YueJia.Ebk.Domain.Dept;

namespace YueJia.Ebk.Application.DeptApp
{
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

            //校验唯一性
            if (await DepartmentRepo.IsAnyAsync(x => x.Name == cmd.Name && x.CompanyId == long.Parse(cmd.CompanyId))) throw new InvalidOperationException($"部门【{cmd.Name}】已存在，请更换！");



            //创建公司
            var entity = DepartmentDo.Create(name: cmd.Name,
                                        parentId: long.Parse(cmd.ParentDeptId),
                                        companyId: long.Parse(cmd.CompanyId),
                                        status: cmd.Status,
                                        tenantId: CurrentUserApp.TenantId.ToLong()) ?? throw new InvalidOperationException("部门创建失败！");

            return await DepartmentRepo.InsertReturnSnowflakeIdAsync(entity);
        }



        public async Task<bool> DeleteDeptAsync(long id)
        {



            // 使用递归CTE查询 
            var entityList = await db.Ado.SqlQueryAsync<DepartmentDo>(@"
                                    WITH cte AS (
                                        SELECT 
                                            id, name, parent_id, company_id, status, 
                                            create_time, createdby_id, createdby_name, 
                                            last_modified_time, last_modifiedby_id, last_modifiedby_name, 
                                            is_delete, version 
                                        FROM 
                                            department 
                                        WHERE 
                                            id = @DepartmentId AND is_delete = 0 
                                        UNION ALL 
                                        SELECT 
                                            d.id,  d.name,  d.parent_id,  d.company_id,  d.status,  
                                            d.create_time,  d.createdby_id,  d.createdby_name,  
                                            d.last_modified_time,  d.last_modifiedby_id,  d.last_modifiedby_name,  
                                            d.is_delete,  d.version  
                                        FROM 
                                            department d 
                                        INNER JOIN 
                                            cte dh ON d.parent_id  = dh.id  
                                        WHERE 
                                            d.is_delete  = 0 
                                    )
                                    SELECT * FROM cte ", new { DepartmentId = id }) ?? throw new InvalidOperationException($"部门ID:{id}资源不存在！");
            // var entity = await DepartmentRepo.GetByIdAsync(id) ?? throw new InvalidOperationException($"部门ID:{id}资源不存在！");

            entityList.ForEach(item => item.IsDelete = true);
            var affectedRows = await DepartmentRepo.AsUpdateable(entityList)
             .UpdateColumns(it => new { it.IsDelete, it.Version, it.LastModifiedbyId, it.LastModifiedbyName, it.LastModifiedTime })
             .ExecuteCommandWithOptLockAsync();
            if (affectedRows is not 1) throw new InvalidOperationException($"部门信息删除失败!");
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
                .Select(x => new SelectDataDto<string>() { Label = x.Name, Text = x.Id.ToString() })
                .ToListAsync();
            result.Insert(0, new SelectDataDto<string>() { Label = "顶级部门", Text = "-1" });
            return result;
        }

        public async Task<PageData<IEnumerable<DeptPageListDto>>> GetPageListDeptAsync(DeptPageListQry qry)
        {
            RefAsync<int> total = 0;

            var query = DepartmentRepo.AsQueryable().With(SqlWith.NoLock)
                   .LeftJoin<CompanyDO>((t, t1) => t.CompanyId == t1.Id).With(SqlWith.NoLock)
                   .WhereIF(!string.IsNullOrWhiteSpace(qry.Name), (t, t1) => SqlFunc.Like(t.Name, $"{qry.Name}%"))
                   .WhereIF(qry.Status.HasValue, (t, t1) => t.Status.Equals(qry.Status))
                   .Where((t, t1) => t.ParentId == -1)
                   .Select((t, t1) => new DeptPageListDto()
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


            var ids = data.Select(x => x.DeptId).ToList();

            var ChildData = await DepartmentRepo.AsQueryable().With(SqlWith.NoLock)
                   .LeftJoin<CompanyDO>((t, t1) => t.CompanyId == t1.Id).With(SqlWith.NoLock)
                   .WhereIF(!string.IsNullOrWhiteSpace(qry.Name), (t, t1) => SqlFunc.Like(t.Name, $"{qry.Name}%"))
                   .WhereIF(qry.Status.HasValue, (t, t1) => t.Status.Equals(qry.Status))
                   .Where((t, t1) => t.ParentId != -1 && ids.Contains(t.ParentId))
                   .Select((t, t1) => new DeptPageListBaseDto()
                   {
                       DeptId = t.Id,
                       DeptName = t.Name,
                       ParentDeptId = t.ParentId,
                       CompanyName = t1.Name,
                       CompanyId = t1.Id,
                       Status = t.Status,
                       CreateTime = t.CreateTime,
                   }).ToListAsync();

            data = data.Select(item =>
            {
                item.Children = ChildData.Where(q => q.ParentDeptId == item.DeptId).ToList();
                return item;
            })
            .ToList();
            return new PageData<IEnumerable<DeptPageListDto>>(total, qry.PageSize, qry.PageIndex, data);
        }

        public async Task<bool> UpdateDeptAsync(CreateOrUpdateDeptCmd cmd, long id)
        {
            //验证更新模型参数
            await LazyServiceProvider.LazyGetRequiredService<FluentValidation.IValidator<CreateOrUpdateDeptCmd>>().ValidateAndThrowAsync(cmd);

            //校验唯一性 排除自己
            if (await DepartmentRepo.IsAnyAsync(x => x.Name == cmd.Name && x.CompanyId == long.Parse(cmd.CompanyId) && x.Id != id)) throw new InvalidOperationException("部门名称已存在，请更换！");


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


    }
}
