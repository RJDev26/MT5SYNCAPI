using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MobileAccounting.Entities;
using MobileAccounting.Repositories.Interfaces;
using OTS.DOMAIN.Database;
using OTS.DOMAIN.MobileAccountingVM;
using OTS.DOMAIN.MobileAccoutingDTO;
using OTS.Infrastructutre.Generic.WebBroker.DataAccessCore;
using System.Data;

namespace MobileAccounting.Repositories.Implementations
{

    public class AccountMasterRepository : IAccountMasterRepository
    {
        private readonly AccountingDbContext _context;
        private readonly DbManager _dbManager;

        public AccountMasterRepository(AccountingDbContext context, DbManager dbManager)
        {
            _context = context;
            _dbManager = dbManager;
        }

        public async Task<string> SaveOrUpdateAccountAsync(AccountMaster model)
        {
            var parameters = new List<DbParameter>
        {
            new DbParameter("accountid", ParameterDirection.Input, model.AccountId),
            new DbParameter("shortcode", ParameterDirection.Input, model.ShortCode),
            new DbParameter("name", ParameterDirection.Input, model.Name),
            new DbParameter("openingbalance", ParameterDirection.Input, model.OpeningBalance),
            new DbParameter("drcr", ParameterDirection.Input, model.drcr),
            new DbParameter("createdby", ParameterDirection.Input, model.CreatedBy),
            new DbParameter("CreatedAt", ParameterDirection.Input, model.CreatedAt),
            new DbParameter("message", ParameterDirection.Output, null)
        };

            await _dbManager.ExecuteNonQueryAsync("usp_SaveOrUpdate_AccountMaster", parameters);

            return _dbManager.OutParameters.FirstOrDefault(x => x.Name.Equals("message", StringComparison.OrdinalIgnoreCase))?.Value?.ToString();
        }
        public async Task<List<AccountMaster>> GetAccountsAsync(int? accountId = null)
        {
            var parameters = new List<DbParameter>
        {
            new DbParameter("AccountId", ParameterDirection.Input, accountId)
        };

            return await _dbManager.ExecuteListAsync<AccountMaster>("usp_Get_AccountMaster", parameters);
        }





        public async Task<string> DeleteAccountAsync(int accountId, int userId)
        {
            var parameters = new List<DbParameter>
    {
        new DbParameter("AccountId", ParameterDirection.Input, accountId),
        new DbParameter("UserId", ParameterDirection.Input, userId),
        new DbParameter("Message", ParameterDirection.Output, null, 255)
    };

            await _dbManager.ExecuteNonQueryAsync("usp_Delete_Account", parameters);

            var message = parameters.First(p => p.Name == "Message").Value?.ToString();
            return message ?? "No message returned";
        }

        public async Task<string> SaveOrUpdateAccountGroupAsync(AccountGroupRequest request)
        {
            var parameters = new List<DbParameter>
    {
        new DbParameter("GroupId", ParameterDirection.InputOutput, request.GroupId ?? 0),
        new DbParameter("GroupCode", ParameterDirection.Input, request.GroupCode),
        new DbParameter("GroupName", ParameterDirection.Input, request.GroupName),
        new DbParameter("Message", ParameterDirection.Output, null, 200)
    };

            await _dbManager.ExecuteNonQueryAsync("usp_SaveOrUpdate_AccountGroup", parameters);

            // Update request.GroupId from output
            request.GroupId = Convert.ToInt32(parameters.First(p => p.Name == "GroupId").Value);
            var message = parameters.First(p => p.Name == "Message").Value?.ToString();

            return message ?? "No message returned";
        }
        public async Task<List<AccountGroupVM>> GetAccountGroupListAsync()
        {
            return await _dbManager.ExecuteListAsync<AccountGroupVM>("usp_Get_AccountGroupList",null);
        }
        public async Task<List<AccountGroupMappingDto>> GetAccountGroupMappingsAsync()
        {
            var result = await _dbManager.ExecuteListAsync<AccountGroupMappingDto>(
                "usp_GetAccountGroupMappings",
                new List<DbParameter>()); // no parameters

            return result;
        }

        public async Task<bool> AddAccountsToGroupAsync(int groupId, string accountIdsCsv)
        {
           

            var parameters = new List<DbParameter>
        {
            new DbParameter("GroupId", ParameterDirection.Input, groupId),
            new DbParameter("AccountIds", ParameterDirection.Input, accountIdsCsv),
           
        };
            await _dbManager.ExecuteNonQueryAsync("usp_AddAccountsToGroup", parameters);
            return true;
        }
        public async Task<List<AccountGroupWithAccountsDto>> GetAccountGroupsWithAccountsAsync(int groupId = 0)
        {
           

            var parameters = new List<DbParameter>
        {
            new DbParameter("GroupId", ParameterDirection.Input, groupId),
         

        };

            var result = await _dbManager.ExecuteListAsync<AccountGroupWithAccountsDto>(
                "usp_GetAccountGroupsWithAccounts", parameters
            );

            return result.ToList();
        }

        public async Task<string> AddOrDeleteAccountsAsync(AccountGroupActionRequest request)
        {
            var parameters = new List<DbParameter>
        {
            new DbParameter("GroupId", ParameterDirection.Input, request.GroupId),
            new DbParameter("AccountIds", ParameterDirection.Input, request.AccountIds),
            new DbParameter("Action", ParameterDirection.Input, request.Action ?? "ADD")
        };

            await _dbManager.ExecuteNonQueryAsync("usp_AddAccountsToGroup", parameters);
            return $"Accounts {request.Action.ToUpper()} operation completed.";
        }

    }

}