using MobileAccounting.Entities;
using MobileAccounting.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileAccounting.Repositories.Implementations
{
    public class AccountGroupMasterRepository : IAccountGroupMasterRepository
    {
        public Task<IEnumerable<AccountGroupMaster>> GetAllAsync() => Task.FromResult<IEnumerable<AccountGroupMaster>>(new List<AccountGroupMaster>());
        public Task<AccountGroupMaster> GetByIdAsync(int id) => Task.FromResult<AccountGroupMaster>(null);
        public Task AddAsync(AccountGroupMaster entity) => Task.CompletedTask;
        public Task UpdateAsync(AccountGroupMaster entity) => Task.CompletedTask;
        public Task DeleteAsync(int id) => Task.CompletedTask;
    }
}