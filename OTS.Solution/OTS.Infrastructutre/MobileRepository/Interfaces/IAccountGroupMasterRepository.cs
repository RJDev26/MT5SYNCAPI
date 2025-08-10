using MobileAccounting.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileAccounting.Repositories.Interfaces
{
    public interface IAccountGroupMasterRepository
    {
        Task<IEnumerable<AccountGroupMaster>> GetAllAsync();
        Task<AccountGroupMaster> GetByIdAsync(int id);
        Task AddAsync(AccountGroupMaster entity);
        Task UpdateAsync(AccountGroupMaster entity);
        Task DeleteAsync(int id);
    }
}