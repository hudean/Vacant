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
    public class CommentService : ICommentService
    {
        private readonly IEfCoreRepository<Comment, int> _commentRepository;

        public CommentService(IEfCoreRepository<Comment, int> commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public Task<bool> AgreeAsync(int id, int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _commentRepository.DeleteAsync(id);
            return await _commentRepository.GetDbContext().SaveChangesAsync() > 0;
        }

        public async Task<PaginatedList<CommentDto>> GetPaginatedListAsync(int userId, int relationId, string replyType, int pageIndex, int pageSize)
        {
            var query = from a in _commentRepository.GetQueryable().Where(x => x.UserId == userId)
                        select new CommentDto()
                        {
                            Id = a.Id,
                            UserId = a.UserId,
                            //FollowedUserId = a.FollowedUserId,
                            CreatedDate = a.CreatedDate,
                        };
            int count = await query.CountAsync();
            var list = new List<CommentDto>();
            if (count >= 0)
            {
                list = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            }
            return new PaginatedList<CommentDto>(list, count, pageIndex, pageSize);
        }

        public async Task<bool> InsertAsync(int userId,string content, int relationId,string relationType)
        {
            var comment = new Comment()
            {
                UserId = userId,
                Content = content,
                IsSetTop = false,
                RelationId = relationId,
                RelationType = relationType,
                LikeCount = 0,
                Removed = false,
                ReplyCount = 0,
                ReviewStatus = 0,
                CreatedDate = DateTime.Now,
            };
            await _commentRepository.InsertAsync(comment);
            return await _commentRepository.GetDbContext().SaveChangesAsync() > 0;
        }
    }
}
