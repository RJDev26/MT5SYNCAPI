using MobileAccounting.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileAccounting.Repositories.Interfaces
{
    public interface IAuditLogRepository
    {
        Task<IEnumerable<AuditLog>> GetAllAsync();
        Task<AuditLog> GetByIdAsync(int id);
        Task AddAsync(AuditLog entity);
        Task UpdateAsync(AuditLog entity);
        Task DeleteAsync(int id);
    }
}