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
    /// 文章服务接口
    /// </summary>
    public interface IArticleService : IBaseService
    {
        /***
         *  1、分页获取文章列表
         *  2、获取我发布/某个用户发布的文章列表
         *  3、文章详情
         *  4、修改文章
         *  5、删除文章
         *  6、添加文章
         * **/

        /// <summary>
        /// 获取文章分页列表
        /// </summary>
        /// <returns></returns>
        Task<PaginatedList<ArticleDto>> GetPaginatedListAsync(string keyWord, string sortType, int topicTypeId, int articleTypeId, int pageIndex, int pageSize);

        Task<PaginatedList<ArticleDto>> GetPaginatedListByUserIdAsync(int userId, int pageIndex, int pageSize);

        Task<PaginatedList<ArticleDto>> GetMyPublishPaginatedListByUserIdAsync(int userId, int pageIndex, int pageSize);

        Task<bool> InsertAsync(int userId, string content, string title, string picUrl, int articleTypeId, int? topicTypeId);
        Task<bool> UpdateAsync(int id, string content, string title, string picUrl, int articleTypeId, int? topicTypeId);

        Task<bool> DeleteAsync(int id);

        Task<ArticleDetailDto> DetailAsync(int id);

        Task<ArticleDto> InsertAndGetEntity(int userId, string content, string title, string picUrl, int articleTypeId, int? topicTypeId);

        Task<ArticleDto> UpdateAndGetEntityAsync(int id, string content, string title, string picUrl, int articleTypeId, int? topicTypeId);
 


    }
}
