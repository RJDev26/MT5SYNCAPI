using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OTS.DOMAIN.MobileAccountingVM;

namespace OTS.Service.Interfaces
{
    public interface IMasterService
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
        Task<MasterResponseVM> ManageClientMasterAsync(ClientMasterRequestVM request, CancellationToken ct);
    }
}
