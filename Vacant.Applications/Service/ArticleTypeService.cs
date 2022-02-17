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
    public class ArticleTypeService : IArticleTypeService
    {
        private readonly IEfCoreRepository<ArticleType, int> _articleTypeRepository;

        public ArticleTypeService(IEfCoreRepository<ArticleType, int> articleTypeRepository)
        {
            _articleTypeRepository = articleTypeRepository;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _articleTypeRepository.DeleteAsync(id);
            return await _articleTypeRepository.GetDbContext().SaveChangesAsync() > 0;
        }

        public async Task<List<ArticleType>> GetListAllAsync()
        {
            var list = new List<ArticleType>();
            var query = from a in _articleTypeRepository.GetQueryable()
                        select new ArticleType()
                        {
                            Id = a.Id,
                            TypeName = a.TypeName,
                            SortNo = a.SortNo,
                        };

            list = await query.ToListAsync();
            return list;
        }

        public async Task<PaginatedList<ArticleType>> GetPaginatedListAsync(int pageIndex, int pageSize)
        {
            var query = from a in _articleTypeRepository.GetQueryable()
                        select new ArticleType()
                        {
                            Id = a.Id,
                            TypeName = a.TypeName,
                            SortNo = a.SortNo,
                        };
            int count = await query.CountAsync();
            var list = new List<ArticleType>();
            if (count >= 0)
            {
                list = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            }
            return new PaginatedList<ArticleType>(list, count, pageIndex, pageSize);
        }

        public async Task<bool> InsertAsync(string typeName, int sortNo)
        {
            var articleType = new ArticleType()
            {
                TypeName = typeName,
                SortNo = sortNo
            };
            await _articleTypeRepository.InsertAsync(articleType);
            return await _articleTypeRepository.GetDbContext().SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(int id, string typeName, int sortNo)
        {
            var articleType = await _articleTypeRepository.GetQueryable().Where(r => r.Id == id).FirstOrDefaultAsync();
            if (articleType != null)
            {
                if (await _articleTypeRepository.GetQueryable().Where(r => r.Id != id && r.TypeName == typeName).AnyAsync())
                {
                    return false;
                }
                articleType.TypeName = typeName;
                articleType.SortNo = sortNo;
                await _articleTypeRepository.UpdateAsync(articleType);
                return await _articleTypeRepository.GetDbContext().SaveChangesAsync() > 0;
            }
            return false;
        }
    }
}
