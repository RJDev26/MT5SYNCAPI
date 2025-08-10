using MobileAccounting.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileAccounting.Repositories.Interfaces
{
    public interface IAccountGroupDetailRepository
    {
        Task<IEnumerable<AccountGroupDetail>> GetAllAsync();
        Task<AccountGroupDetail> GetByIdAsync(int id);
        Task AddAsync(AccountGroupDetail entity);
        Task UpdateAsync(AccountGroupDetail entity);
        Task DeleteAsync(int id);
    }
}