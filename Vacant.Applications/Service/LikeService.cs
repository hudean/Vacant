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
    public class LikeService : ILikeService
    {
        public readonly IEfCoreRepository<Like, int> _likeRepository;


        public LikeService(IEfCoreRepository<Like, int> likeRepository)
        {
            _likeRepository = likeRepository;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _likeRepository.DeleteAsync(id);
            return await _likeRepository.GetDbContext().SaveChangesAsync() > 0;
        }

        public async Task<PaginatedList<LikeDto>> GetPaginatedListAsync(int userId, int pageIndex, int pageSize)
        {
            var query = from c in _likeRepository.GetQueryable().Where(x => x.UserId == userId)
                        select new LikeDto()
                        {
                            Id = c.Id,
                            UserId = c.UserId,
                            RelationId = c.RelationId,
                            RelationType = c.RelationType,
                            CreatedDate = c.CreatedDate,
                        };
            int count = await query.CountAsync();
            var list = new List<LikeDto>();
            if (count >= 0)
            {
                list = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            }
            return new PaginatedList<LikeDto>(list, count, pageIndex, pageSize);
        }

        public async Task<bool> InsertAsync(int userId, int relationId, string relationType)
        {
            var like = new Like()
            {
                UserId = userId,
                RelationId = relationId,
                RelationType = relationType,
                CreatedDate = DateTime.Now,
            };
            await _likeRepository.InsertAsync(like);
            return await _likeRepository.GetDbContext().SaveChangesAsync() > 0;
        }
    }
}
