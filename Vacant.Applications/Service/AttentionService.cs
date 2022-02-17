using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vacant.Applications.IService;
using Vacant.Domain.Entites;
using Vacant.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using Vacant.Model;
using Vacant.Model.Dto;

namespace Vacant.Applications.Service
{
    public class AttentionService : IAttentionService
    {
        public readonly IEfCoreRepository<Attention, int> _attentionRepository;


        public AttentionService(IEfCoreRepository<Attention, int> attentionRepository)
        {
            _attentionRepository = attentionRepository;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _attentionRepository.DeleteAsync(id);
            return await _attentionRepository.GetDbContext().SaveChangesAsync() > 0;
        }

        public async Task<PaginatedList<AttentionDto>> GetPaginatedListAsync(int userId, int pageIndex, int pageSize)
        {
            var query = from a in _attentionRepository.GetQueryable().Where(x => x.UserId == userId)
                        select new AttentionDto()
                        {
                            Id = a.Id,
                            UserId = a.UserId,
                            FollowedUserId = a.FollowedUserId,
                            CreatedDate = a.CreatedDate,
                        };
            int count = await query.CountAsync();
            var list = new List<AttentionDto>();
            if (count >= 0)
            {
                list = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            }
            return new PaginatedList<AttentionDto>(list, count, pageIndex,pageSize);
        }

        public async Task<bool> InsertAsync(int userId, int followedUserId)
        {
            var attention = new Attention()
            {
                UserId = userId,
                FollowedUserId = followedUserId,
                CreatedDate = DateTime.Now,
            };
            await _attentionRepository.InsertAsync(attention);
            return await _attentionRepository.GetDbContext().SaveChangesAsync() > 0;
            //var result =  await _attentionRepository.InsertAndGetEntityAsync(attention,true);
            //return result != null && result.Id > 0;

        }
    }
}
