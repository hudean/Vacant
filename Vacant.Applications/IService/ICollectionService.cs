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
    /// 收藏服务接口
    /// </summary>
    public interface ICollectionService : IBaseService
    {
        Task<PaginatedList<CollectionDto>> GetPaginatedListAsync(int userId, int pageIndex, int pageSize);

        Task<bool> InsertAsync(int userId, int relationId, string relationType);


        Task<bool> DeleteAsync(int id);

    }
}
