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

        public Task<List<MasterListVM>> GetMasterListAsync(string tableName, CancellationToken ct)
        {
            var parameters = new List<DbParameter>
            {
                new DbParameter("TableName", ParameterDirection.Input, tableName)
            };

            return _db.ExecuteListAsync<MasterListVM>("usp_Get_MasterList", parameters);
        }

        public Task<List<LoginVM>> GetLoginsAsync(CancellationToken ct)
            => _db.ExecuteListAsync<LoginVM>("usp_Get_Logins", null);

        public Task<List<LoginClientInfoVM>> GetMt5LoginsWithClientInfoAsync(
            long? login,
            int? managerId,
            int? brokerId,
            int? exId,
            bool onlyWithClientRecord,
            CancellationToken ct)
        {
            var parameters = new List<DbParameter>
            {
                new DbParameter("Login", ParameterDirection.Input, login),
                new DbParameter("ManagerId", ParameterDirection.Input, managerId),
                new DbParameter("BrokerId", ParameterDirection.Input, brokerId),
                new DbParameter("ExId", ParameterDirection.Input, exId),
                new DbParameter("OnlyWithClientRecord", ParameterDirection.Input, onlyWithClientRecord)
            };

            return _db.ExecuteListAsync<LoginClientInfoVM>("usp_GetMt5LoginsWithClientInfo", parameters);
        }

        public async Task<MasterResponseVM> ManageClientMasterAsync(ClientMasterRequestVM request, CancellationToken ct)
        {
            request.ExId = string.Join(",", request.ExIds); 
            var parameters = new List<DbParameter>
            {
                new DbParameter("Action", ParameterDirection.Input, request.Action),
                new DbParameter("Id", ParameterDirection.Input, request.Id),
                new DbParameter("Login", ParameterDirection.Input, request.Login),
                new DbParameter("ManagerId", ParameterDirection.Input, request.ManagerId),
                new DbParameter("BrokerId", ParameterDirection.Input, request.BrokerId),
                new DbParameter("ExId", ParameterDirection.Input, request.ExId),
                new DbParameter("BrokShare", ParameterDirection.Input, request.BrokShare),
                new DbParameter("ManagerShare", ParameterDirection.Input, request.ManagerShare),
                new DbParameter("Currency", ParameterDirection.Input, request.CurrencyId),
                new DbParameter("Commission", ParameterDirection.Input, request.Commission),
                new DbParameter("CreatedBy", ParameterDirection.Input, request.CreatedBy)
            };

            var result = await _db.ExecuteListAsync<MasterResponseVM>("usp_ManageClientMaster", parameters);
            return result.FirstOrDefault() ?? new MasterResponseVM();
        }
    }
}
