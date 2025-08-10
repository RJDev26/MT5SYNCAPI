using MobileAccounting.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileAccounting.Repositories.Interfaces
{
    public interface IVoucherMasterDeletedRepository
    {
        Task<IEnumerable<VoucherMasterDeleted>> GetAllAsync();
        Task<VoucherMasterDeleted> GetByIdAsync(int id);
        Task AddAsync(VoucherMasterDeleted entity);
        Task UpdateAsync(VoucherMasterDeleted entity);
        Task DeleteAsync(int id);
    }
}