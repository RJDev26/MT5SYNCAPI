using MobileAccounting.Entities;
using MobileAccounting.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileAccounting.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        public Task<IEnumerable<User>> GetAllAsync() => Task.FromResult<IEnumerable<User>>(new List<User>());
        public Task<User> GetByIdAsync(int id) => Task.FromResult<User>(null);
        public Task AddAsync(User entity) => Task.CompletedTask;
        public Task UpdateAsync(User entity) => Task.CompletedTask;
        public Task DeleteAsync(int id) => Task.CompletedTask;
    }
}