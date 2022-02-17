using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vacant.Applications.IService;
using Vacant.Domain.Entites;
using Vacant.EntityFrameworkCore.Repositories;
using Vacant.Model;
using Vacant.Model.Dto;

namespace Vacant.Applications.Service
{
    public class ArticleService : IArticleService
    {

        public readonly IEfCoreRepository<Article, int> _articleRepository;

        public ArticleService(IEfCoreRepository<Article, int> articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _articleRepository.DeleteAsync(id);
            return await _articleRepository.GetDbContext().SaveChangesAsync() > 0;
        }

        public async Task<ArticleDetailDto> DetailAsync(int id)
        {
            var article = await _articleRepository.FindAsync(id);

            ArticleDetailDto articleDetailDto = new ArticleDetailDto();

            return articleDetailDto;
        }

        public async Task<PaginatedList<ArticleDto>> GetPaginatedListAsync(string keyWord, string sortType, int topicTypeId, int articleTypeId, int pageIndex, int pageSize)
        {
            var context = _articleRepository.GetDbContext();
            var query = from a in context.Set<Article>().AsQueryable().Where(r => r.ReviewStatus == 1)
                        join u in context.Set<User>() on a.UserId equals u.Id
                        select new ArticleDto()
                        {

                        };
            int count = await query.CountAsync();
            var list = new List<ArticleDto>();
            if (count >= 0)
            {
                list = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            }
            return new PaginatedList<ArticleDto>(list, count, pageIndex, pageSize);
        }


        public async Task<PaginatedList<ArticleDto>> GetPaginatedListByUserIdAsync(int userId, int pageIndex, int pageSize)
        {
            var context = _articleRepository.GetDbContext();
            var query = from a in context.Set<Article>().AsQueryable().Where(r => r.ReviewStatus == 1)
                        join u in context.Set<User>() on a.UserId equals u.Id
                        select new ArticleDto()
                        {

                        };
            int count = await query.CountAsync();
            var list = new List<ArticleDto>();
            if (count >= 0)
            {
                list = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            }
            return new PaginatedList<ArticleDto>(list, count, pageIndex, pageSize);
        }

        /// <summary>
        /// 获取我发布的文章列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<PaginatedList<ArticleDto>> GetMyPublishPaginatedListByUserIdAsync(int userId, int pageIndex, int pageSize)
        {
            var context = _articleRepository.GetDbContext();
            var query = from a in context.Set<Article>().AsQueryable().Where(r => r.ReviewStatus == 1)
                        join u in context.Set<User>() on a.UserId equals u.Id
                        select new ArticleDto()
                        {

                        };
            int count = await query.CountAsync();
            var list = new List<ArticleDto>();
            if (count >= 0)
            {
                list = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            }
            return new PaginatedList<ArticleDto>(list, count, pageIndex, pageSize);
        }


        public async Task<ArticleDto> InsertAndGetEntity(int userId, string content, string title, string picUrl, int articleTypeId, int? topicTypeId)
        {
            var entity = new Article()
            {
                UserId = userId,
                Content = content,
                Title = title,
                PicUrl = picUrl,
                ArticleTypeId = articleTypeId,
                TopicTypeId = topicTypeId,
                CreatedDate = DateTime.Now,
            };
            entity = await _articleRepository.InsertAndGetEntityAsync(entity);
            await _articleRepository.GetDbContext().SaveChangesAsync();
            return null;
        }

        public async Task<bool> InsertAsync(int userId, string content, string title, string picUrl, int articleTypeId, int? topicTypeId)
        {
            var entity = new Article()
            {
                UserId = userId,
                Content = content,
                Title = title,
                PicUrl = picUrl,
                ArticleTypeId = articleTypeId,
                TopicTypeId = topicTypeId,
                CreatedDate = DateTime.Now,
            };
            await _articleRepository.InsertAsync(entity);
            return await _articleRepository.GetDbContext().SaveChangesAsync() > 0;
        }

        public async Task<ArticleDto> UpdateAndGetEntityAsync(int id, string content, string title, string picUrl, int articleTypeId, int? topicTypeId)
        {
            var article = await _articleRepository.FindAsync(r => r.Id == id);
            if (article != null)
            {
                article.UpdatedDate = DateTime.Now;
                article.Title = title;
                article.ArticleTypeId = articleTypeId;
                article.Content = content;
                article.PicUrl = picUrl;
                article.TopicTypeId = topicTypeId;
                await _articleRepository.UpdateAsync(article);
                await _articleRepository.GetDbContext().SaveChangesAsync();
            }

            return null;
        }

        public async Task<bool> UpdateAsync(int id, string content, string title, string picUrl, int articleTypeId, int? topicTypeId)
        {
            var article = await _articleRepository.FindAsync(r => r.Id == id);
            if (article != null)
            {
                article.UpdatedDate = DateTime.Now;
                article.Title = title;
                article.ArticleTypeId = articleTypeId;
                article.Content = content;
                article.PicUrl = picUrl;
                //article.UserId = userId;
                article.TopicTypeId = topicTypeId;
                await _articleRepository.UpdateAsync(article);

                return await _articleRepository.GetDbContext().SaveChangesAsync() > 0;
            }

            return false;
        }
    }
}
