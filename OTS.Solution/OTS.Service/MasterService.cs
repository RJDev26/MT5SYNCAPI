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
    }
}
