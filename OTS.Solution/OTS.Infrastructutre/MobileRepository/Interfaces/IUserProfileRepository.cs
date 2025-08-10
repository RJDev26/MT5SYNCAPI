using MobileAccounting.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileAccounting.Repositories.Interfaces
{
    public interface IUserProfileRepository
    {
        Task<IEnumerable<UserProfile>> GetAllAsync();
        Task<UserProfile> GetByIdAsync(int id);
        Task AddAsync(UserProfile entity);
        Task UpdateAsync(UserProfile entity);
        Task DeleteAsync(int id);
    }
}