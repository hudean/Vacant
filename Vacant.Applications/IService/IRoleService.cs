using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vacant.Model;
using Vacant.Model.Dto;

namespace Vacant.Applications.IService
{
    /// <summary>
    /// 角色服务接口
    /// </summary>
    public interface IRoleService : IBaseService
    {

        Task<List<RoleDto>> GetListAllAsync();

        Task<PaginatedList<RoleDto>> GetPaginatedListAsync(int pageIndex, int pageSize);
        Task<bool> InsertAsync(RoleDto dto);

        Task<bool> UpdateAsync(RoleDto dto);
        Task<bool> DeleteAsync(int id);


        Task<List<RoleDto>> GetListAllByUserIdAsync(int userId);

        Task<bool> InsertByUserIdAsync();

        Task<bool> DeleteByUserIdAsync(int userId);
    }
}
