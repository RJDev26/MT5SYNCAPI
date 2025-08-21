using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MobileAccounting.Repositories.Interfaces;
using OTS.DOMAIN.MobileAccountingVM;
using OTS.Service.Interfaces;

namespace OTS.Service
{
    public class MasterService : IMasterService
    {
        private readonly IMasterRepository _repository;

        public MasterService(IMasterRepository repository)
        {
            _repository = repository;
        }

        public Task<MasterResponseVM> SaveOrUpdateAsync(MasterRequestVM request, CancellationToken ct)
            => _repository.SaveOrUpdateAsync(request, ct);

        public Task<MasterResponseVM> DeleteAsync(string tableName, int id, CancellationToken ct)
            => _repository.DeleteAsync(tableName, id, ct);

        public Task<List<MasterListVM>> GetMasterListAsync(string tableName, CancellationToken ct)
            => _repository.GetMasterListAsync(tableName, ct);

        public Task<List<LoginVM>> GetLoginsAsync(CancellationToken ct)
            => _repository.GetLoginsAsync(ct);

        public Task<List<LoginClientInfoVM>> GetMt5LoginsWithClientInfoAsync(
            long? login,
            int? managerId,
            int? brokerId,
            int? exId,
            bool onlyWithClientRecord,
            CancellationToken ct)
            => _repository.GetMt5LoginsWithClientInfoAsync(login, managerId, brokerId, exId, onlyWithClientRecord, ct);

        public Task<MasterResponseVM> ManageClientMasterAsync(ClientMasterRequestVM request, CancellationToken ct)
            => _repository.ManageClientMasterAsync(request, ct);
    }
}
