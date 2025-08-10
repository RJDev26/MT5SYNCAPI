using MobileAccounting.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileAccounting.Repositories.Interfaces
{
    public interface IRolePermissionRepository
    {
        Task<IEnumerable<RolePermission>> GetAllAsync();
        Task<RolePermission> GetByIdAsync(int id);
        Task AddAsync(RolePermission entity);
        Task UpdateAsync(RolePermission entity);
        Task DeleteAsync(int id);
    }
}