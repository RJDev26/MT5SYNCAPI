using MobileAccounting.Entities;
using MobileAccounting.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileAccounting.Repositories.Implementations
{
    public class AuditLogRepository : IAuditLogRepository
    {
        public Task<IEnumerable<AuditLog>> GetAllAsync() => Task.FromResult<IEnumerable<AuditLog>>(new List<AuditLog>());
        public Task<AuditLog> GetByIdAsync(int id) => Task.FromResult<AuditLog>(null);
        public Task AddAsync(AuditLog entity) => Task.CompletedTask;
        public Task UpdateAsync(AuditLog entity) => Task.CompletedTask;
        public Task DeleteAsync(int id) => Task.CompletedTask;
    }
}