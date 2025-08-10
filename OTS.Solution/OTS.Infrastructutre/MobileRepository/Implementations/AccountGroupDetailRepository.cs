using MobileAccounting.Entities;
using MobileAccounting.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileAccounting.Repositories.Implementations
{
    public class AccountGroupDetailRepository : IAccountGroupDetailRepository
    {
        public Task<IEnumerable<AccountGroupDetail>> GetAllAsync() => Task.FromResult<IEnumerable<AccountGroupDetail>>(new List<AccountGroupDetail>());
        public Task<AccountGroupDetail> GetByIdAsync(int id) => Task.FromResult<AccountGroupDetail>(null);
        public Task AddAsync(AccountGroupDetail entity) => Task.CompletedTask;
        public Task UpdateAsync(AccountGroupDetail entity) => Task.CompletedTask;
        public Task DeleteAsync(int id) => Task.CompletedTask;
    }
}