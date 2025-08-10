using MobileAccounting.Entities;
using MobileAccounting.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileAccounting.Repositories.Implementations
{
    public class VoucherMasterDeletedRepository : IVoucherMasterDeletedRepository
    {
        public Task<IEnumerable<VoucherMasterDeleted>> GetAllAsync() => Task.FromResult<IEnumerable<VoucherMasterDeleted>>(new List<VoucherMasterDeleted>());
        public Task<VoucherMasterDeleted> GetByIdAsync(int id) => Task.FromResult<VoucherMasterDeleted>(null);
        public Task AddAsync(VoucherMasterDeleted entity) => Task.CompletedTask;
        public Task UpdateAsync(VoucherMasterDeleted entity) => Task.CompletedTask;
        public Task DeleteAsync(int id) => Task.CompletedTask;
    }
}