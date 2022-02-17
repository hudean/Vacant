using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vacant.Domain.Entites;
using Vacant.Model;
using Vacant.Model.Dto;

namespace Vacant.Applications.IService
{
    /// <summary>
    /// 关注服务接口
    /// </summary>
    public interface IAttentionService : IBaseService
    {

        Task<PaginatedList<AttentionDto>> GetPaginatedListAsync(int userId, int pageIndex, int pageSize);

        Task<bool> InsertAsync(int userId, int followedUserId);


        Task<bool> DeleteAsync(int id);



    }
}
