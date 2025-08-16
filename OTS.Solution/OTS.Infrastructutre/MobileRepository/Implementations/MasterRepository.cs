using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MobileAccounting.Repositories.Interfaces;
using OTS.DOMAIN.MobileAccountingVM;
using OTS.Infrastructutre.Generic.WebBroker.DataAccessCore;

namespace MobileAccounting.Repositories.Implementations
{
    public class MasterRepository : IMasterRepository
    {
        private readonly DbManager _db;

        public MasterRepository(DbManager db)
        {
            _db = db;
        }

        public async Task<MasterResponseVM> SaveOrUpdateAsync(MasterRequestVM request, CancellationToken ct)
        {
            var parameters = new List<DbParameter>
            {
                new DbParameter("TableName", ParameterDirection.Input, request.TableName),
                new DbParameter("Id", ParameterDirection.Input, request.Id),
                new DbParameter("Code", ParameterDirection.Input, request.Code),
                new DbParameter("Name", ParameterDirection.Input, request.Name)
            };

            var result = await _db.ExecuteListAsync<MasterResponseVM>("usp_SaveOrUpdate_Master", parameters);
            return result.FirstOrDefault() ?? new MasterResponseVM();
        }

        public async Task<MasterResponseVM> DeleteAsync(string tableName, int id, CancellationToken ct)
        {
            var parameters = new List<DbParameter>
            {
                new DbParameter("TableName", ParameterDirection.Input, tableName),
                new DbParameter("Id", ParameterDirection.Input, id)
            };

            var result = await _db.ExecuteListAsync<MasterResponseVM>("usp_Delete_Master", parameters);
            return result.FirstOrDefault() ?? new MasterResponseVM();
        }

        public Task<IEnumerable<MasterListVM>> GetMasterListAsync(string tableName, CancellationToken ct)
        {
            var parameters = new List<DbParameter>
            {
                new DbParameter("TableName", ParameterDirection.Input, tableName)
            };

            return _db.ExecuteListAsync<MasterListVM>("usp_Get_MasterList", parameters);
        }

        public Task<IEnumerable<LoginVM>> GetLoginsAsync(CancellationToken ct)
            => _db.ExecuteListAsync<LoginVM>("usp_Get_Logins", null);
    }
}
