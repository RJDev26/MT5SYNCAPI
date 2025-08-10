using MobileAccounting.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileAccounting.Repositories.Interfaces
{
    public interface IVoucherDetailsDeletedRepository
    {
        Task<IEnumerable<VoucherDetailsDeleted>> GetAllAsync();
        Task<VoucherDetailsDeleted> GetByIdAsync(int id);
        Task AddAsync(VoucherDetailsDeleted entity);
        Task UpdateAsync(VoucherDetailsDeleted entity);
        Task DeleteAsync(int id);
    }
}