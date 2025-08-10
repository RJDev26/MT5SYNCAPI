using MobileAccounting.Entities;
using MobileAccounting.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileAccounting.Repositories.Implementations
{
    public class UserRoleRepository : IUserRoleRepository
    {
        public Task<IEnumerable<UserRole>> GetAllAsync() => Task.FromResult<IEnumerable<UserRole>>(new List<UserRole>());
        public Task<UserRole> GetByIdAsync(int id) => Task.FromResult<UserRole>(null);
        public Task AddAsync(UserRole entity) => Task.CompletedTask;
        public Task UpdateAsync(UserRole entity) => Task.CompletedTask;
        public Task DeleteAsync(int id) => Task.CompletedTask;
    }
}