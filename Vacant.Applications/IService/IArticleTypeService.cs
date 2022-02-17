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
    public interface IArticleTypeService : IBaseService
    {
        Task<List<ArticleType>> GetListAllAsync();

        Task<PaginatedList<ArticleType>> GetPaginatedListAsync(int pageIndex, int pageSize);

        Task<bool> InsertAsync(string typeName, int sortNo);

        Task<bool> UpdateAsync(int id, string typeName, int sortNo);
        Task<bool> DeleteAsync(int id);

    }
}
