using MobileAccounting.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileAccounting.Repositories.Interfaces
{
    public interface IPermissionRepository
    {
        Task<IEnumerable<Permission>> GetAllAsync();
        Task<Permission> GetByIdAsync(int id);
        Task AddAsync(Permission entity);
        Task UpdateAsync(Permission entity);
        Task DeleteAsync(int id);
    }
}