using MobileAccounting.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileAccounting.Repositories.Interfaces
{
    public interface IVoucherMasterRepository
    {
        Task<IEnumerable<VoucherMaster>> GetAllAsync();
        Task<VoucherMaster> GetByIdAsync(int id);
        Task AddAsync(VoucherMaster entity);
        Task UpdateAsync(VoucherMaster entity);
        Task DeleteAsync(int id);
    }
}