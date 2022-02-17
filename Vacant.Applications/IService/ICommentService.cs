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
    /// 一级评论服务接口
    /// </summary>
    public interface ICommentService : IBaseService
    {
        Task<PaginatedList<CommentDto>> GetPaginatedListAsync(int userId, int relationId, string replyType, int pageIndex, int pageSize);

        Task<bool> InsertAsync(int userId, string content, int relationId, string relationType);


        Task<bool> DeleteAsync(int id);

        Task<bool> AgreeAsync(int id, int userId);
    }
}
