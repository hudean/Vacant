using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vacant.Applications.IService;
using Vacant.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Vacant.EntityFrameworkCore.Repositories;
using Vacant.Model;
using Vacant.Model.Dto;

namespace Vacant.Applications.Service
{
    public class RoleService : IRoleService
    {
        private readonly IEfCoreRepository<Role, int> _roleRepository;

        private readonly IEfCoreRepository<UserRole, int> _userRoleRepository;

        public RoleService(IEfCoreRepository<Role, int> roleRepository, IEfCoreRepository<UserRole, int> userRoleRepository)
        {
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
        }

        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(int id)
        {
            await _roleRepository.DeleteAsync(id);
            return await _roleRepository.GetDbContext().SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteByUserIdAsync(int userId)
        {
            await _userRoleRepository.DeleteAsync(userId);
            return await _roleRepository.GetDbContext().SaveChangesAsync() > 0;
        }

        public async Task<List<RoleDto>> GetListAllAsync()
        {
            var list = new List<RoleDto>();
            var query = from r in _roleRepository.GetQueryable()
                        select new RoleDto()
                        {
                            Id = r.Id,
                            RoleName = r.RoleName,
                            Description = r.Description,
                        };
            list = await query.ToListAsync();
          
            return list;
        }

        public async Task<List<RoleDto>> GetListAllByUserIdAsync(int userId)
        {
            var list = new List<RoleDto>();
            var query = from r in _roleRepository.GetQueryable()
                        join ur in _userRoleRepository.GetQueryable().Where(u => u.Id == userId) on r.Id equals ur.RoleId
                        select new RoleDto()
                        {
                            Id = r.Id,
                            RoleName = r.RoleName,
                            Description = r.Description,
                        };
            list = await query.ToListAsync();

            return list;
        }


        /// <summary>
        /// 分页获取所有权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<PaginatedList<RoleDto>> GetPaginatedListAsync(int pageIndex, int pageSize)
        {
            var query = from r in _roleRepository.GetQueryable()
                        select new RoleDto()
                        {
                            Id = r.Id,
                            RoleName = r.RoleName,
                            Description = r.Description,
                        };
            int count = await query.CountAsync();
            var list = new List<RoleDto>();
            if (count >= 0)
            {
                list = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            }
            return new PaginatedList<RoleDto>(list, count, pageIndex, pageSize);
        }

        /// <summary>
        /// 插入权限
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<bool> InsertAsync(RoleDto dto)
        {
            if (await _roleRepository.GetQueryable().Where(r => r.RoleName == dto.RoleName).AnyAsync())
            {
                return false;
            }
            var role = new Role()
            {
                RoleName = dto.RoleName,
                Description = dto.Description,
            };
            await _roleRepository.InsertAsync(role);
            return await _roleRepository.GetDbContext().SaveChangesAsync() > 0;
        }

        public Task<bool> InsertByUserIdAsync()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 修改权限
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(RoleDto dto)
        {
            var role = await _roleRepository.GetQueryable().Where(r => r.Id == dto.Id).FirstOrDefaultAsync();
            if (role != null)
            {
                if (await _roleRepository.GetQueryable().Where(r => r.Id != dto.Id && r.RoleName == dto.RoleName).AnyAsync())
                {
                    return false;
                }
                role.RoleName = dto.RoleName;
                role.Description = dto.Description;
                await _roleRepository.UpdateAsync(role);
                return await _roleRepository.GetDbContext().SaveChangesAsync() > 0;
            }
            return false;
        }
    }
}
