using MobileAccounting.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileAccounting.Repositories.Interfaces
{
    public interface IUserRoleRepository
    {
        Task<IEnumerable<UserRole>> GetAllAsync();
        Task<UserRole> GetByIdAsync(int id);
        Task AddAsync(UserRole entity);
        Task UpdateAsync(UserRole entity);
        Task DeleteAsync(int id);
    }
}