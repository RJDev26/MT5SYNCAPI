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
        Task<List<MasterListVM>> GetMasterListAsync(string tableName, CancellationToken ct);
        Task<List<LoginVM>> GetLoginsAsync(CancellationToken ct);
        Task<List<LoginClientInfoVM>> GetMt5LoginsWithClientInfoAsync(
            long? login,
            int? managerId,
            int? brokerId,
            int? exId,
            bool onlyWithClientRecord,
            CancellationToken ct);
    }
}
