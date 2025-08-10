using MobileAccounting.Entities;
using MobileAccounting.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileAccounting.Repositories.Implementations
{
    public class RolePermissionRepository : IRolePermissionRepository
    {
        public Task<IEnumerable<RolePermission>> GetAllAsync() => Task.FromResult<IEnumerable<RolePermission>>(new List<RolePermission>());
        public Task<RolePermission> GetByIdAsync(int id) => Task.FromResult<RolePermission>(null);
        public Task AddAsync(RolePermission entity) => Task.CompletedTask;
        public Task UpdateAsync(RolePermission entity) => Task.CompletedTask;
        public Task DeleteAsync(int id) => Task.CompletedTask;
    }
}