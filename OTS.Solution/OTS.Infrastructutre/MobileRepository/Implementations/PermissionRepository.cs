using MobileAccounting.Entities;
using MobileAccounting.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileAccounting.Repositories.Implementations
{
    public class PermissionRepository : IPermissionRepository
    {
        public Task<IEnumerable<Permission>> GetAllAsync() => Task.FromResult<IEnumerable<Permission>>(new List<Permission>());
        public Task<Permission> GetByIdAsync(int id) => Task.FromResult<Permission>(null);
        public Task AddAsync(Permission entity) => Task.CompletedTask;
        public Task UpdateAsync(Permission entity) => Task.CompletedTask;
        public Task DeleteAsync(int id) => Task.CompletedTask;
    }
}