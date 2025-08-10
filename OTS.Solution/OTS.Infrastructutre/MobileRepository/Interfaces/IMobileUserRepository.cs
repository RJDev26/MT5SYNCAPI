using MobileAccounting.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileAccounting.Repositories.Interfaces
{
    public interface IMobileUserRepository
    {
        Task<IEnumerable<MobileUser>> GetAllAsync();
        Task<MobileUser> GetByIdAsync(int id);
        Task AddAsync(MobileUser entity);
        Task UpdateAsync(MobileUser entity);
        Task DeleteAsync(int id);
    }
}