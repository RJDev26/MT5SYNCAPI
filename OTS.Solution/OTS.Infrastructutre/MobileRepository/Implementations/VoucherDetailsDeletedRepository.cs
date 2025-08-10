using MobileAccounting.Entities;
using MobileAccounting.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileAccounting.Repositories.Implementations
{
    public class VoucherDetailsDeletedRepository : IVoucherDetailsDeletedRepository
    {
        public Task<IEnumerable<VoucherDetailsDeleted>> GetAllAsync() => Task.FromResult<IEnumerable<VoucherDetailsDeleted>>(new List<VoucherDetailsDeleted>());
        public Task<VoucherDetailsDeleted> GetByIdAsync(int id) => Task.FromResult<VoucherDetailsDeleted>(null);
        public Task AddAsync(VoucherDetailsDeleted entity) => Task.CompletedTask;
        public Task UpdateAsync(VoucherDetailsDeleted entity) => Task.CompletedTask;
        public Task DeleteAsync(int id) => Task.CompletedTask;
    }
}