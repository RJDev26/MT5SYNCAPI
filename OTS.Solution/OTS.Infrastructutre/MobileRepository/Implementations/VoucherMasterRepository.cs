using MobileAccounting.Entities;
using MobileAccounting.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileAccounting.Repositories.Implementations
{
    public class VoucherMasterRepository : IVoucherMasterRepository
    {
        public Task<IEnumerable<VoucherMaster>> GetAllAsync() => Task.FromResult<IEnumerable<VoucherMaster>>(new List<VoucherMaster>());
        public Task<VoucherMaster> GetByIdAsync(int id) => Task.FromResult<VoucherMaster>(null);
        public Task AddAsync(VoucherMaster entity) => Task.CompletedTask;
        public Task UpdateAsync(VoucherMaster entity) => Task.CompletedTask;
        public Task DeleteAsync(int id) => Task.CompletedTask;
    }
}