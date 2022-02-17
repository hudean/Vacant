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
    public class CollectionService : ICollectionService
    {
        public readonly IEfCoreRepository<Collection, int> _collectionRepository;

        public CollectionService(IEfCoreRepository<Collection, int> collectionRepository)
        {
            _collectionRepository = collectionRepository;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _collectionRepository.DeleteAsync(id);
            return await _collectionRepository.GetDbContext().SaveChangesAsync() > 0;
        }

        public async Task<PaginatedList<CollectionDto>> GetPaginatedListAsync(int userId, int pageIndex, int pageSize)
        {
            var query = from c in _collectionRepository.GetQueryable().Where(x => x.UserId == userId)
                        select new CollectionDto()
                        {
                            Id = c.Id,
                            UserId = c.UserId,
                            RelationId = c.RelationId,
                            RelationType = c.RelationType,
                            CreatedDate = c.CreatedDate,
                        };
            int count = await query.CountAsync();
            var list = new List<CollectionDto>();
            if (count >= 0)
            {
                list = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            }
            return new PaginatedList<CollectionDto>(list, count, pageIndex, pageSize);
        }

        public async Task<bool> InsertAsync(int userId, int relationId, string relationType)
        {
            var collection = new Collection()
            {
                UserId = userId,
                RelationId = relationId,
                RelationType = relationType,
                CreatedDate = DateTime.Now,
            };
            await _collectionRepository.InsertAsync(collection);
            return await _collectionRepository.GetDbContext().SaveChangesAsync() > 0;
        }

      
    }
}
