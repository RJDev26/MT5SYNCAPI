using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OTS.DOMAIN.MobileAccountingVM;

namespace MobileAccounting.Repositories.Interfaces
{
    public interface IMasterRepository
    {
        Task<MasterResponseVM> SaveOrUpdateAsync(MasterRequestVM request, CancellationToken ct);
        Task<MasterResponseVM> DeleteAsync(string tableName, int id, CancellationToken ct);
        Task<IEnumerable<MasterListVM>> GetMasterListAsync(string tableName, CancellationToken ct);
        Task<IEnumerable<LoginVM>> GetLoginsAsync(CancellationToken ct);
    }
}
