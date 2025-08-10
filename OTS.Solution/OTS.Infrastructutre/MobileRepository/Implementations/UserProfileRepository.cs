using MobileAccounting.Entities;
using MobileAccounting.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileAccounting.Repositories.Implementations
{
    public class UserProfileRepository : IUserProfileRepository
    {
        public Task<IEnumerable<UserProfile>> GetAllAsync() => Task.FromResult<IEnumerable<UserProfile>>(new List<UserProfile>());
        public Task<UserProfile> GetByIdAsync(int id) => Task.FromResult<UserProfile>(null);
        public Task AddAsync(UserProfile entity) => Task.CompletedTask;
        public Task UpdateAsync(UserProfile entity) => Task.CompletedTask;
        public Task DeleteAsync(int id) => Task.CompletedTask;
    }
}