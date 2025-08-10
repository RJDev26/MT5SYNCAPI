using MobileAccounting.Entities;
using MobileAccounting.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileAccounting.Repositories.Implementations
{
    public class MobileUserRepository : IMobileUserRepository
    {
        public Task<IEnumerable<MobileUser>> GetAllAsync() => Task.FromResult<IEnumerable<MobileUser>>(new List<MobileUser>());
        public Task<MobileUser> GetByIdAsync(int id) => Task.FromResult<MobileUser>(null);
        public Task AddAsync(MobileUser entity) => Task.CompletedTask;
        public Task UpdateAsync(MobileUser entity) => Task.CompletedTask;
        public Task DeleteAsync(int id) => Task.CompletedTask;
    }
}