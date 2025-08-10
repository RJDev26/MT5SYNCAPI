using Microsoft.Identity.Client;
using MobileAccounting.Entities;
using OTS.DOMAIN.MobileAccountingVM;
using OTS.DOMAIN.MobileAccoutingDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileAccounting.Repositories.Interfaces
{
    public interface IAccountMasterRepository
    {
       
        Task<string> SaveOrUpdateAccountAsync(AccountMaster model);
        Task<List<AccountMaster>> GetAccountsAsync(int? accountId = null);
         Task<string> SaveOrUpdateAccountGroupAsync(AccountGroupRequest request);
         Task<List<AccountGroupVM>> GetAccountGroupListAsync();
        Task<List<AccountGroupMappingDto>> GetAccountGroupMappingsAsync();
        Task<bool> AddAccountsToGroupAsync(int groupId, string accountIdsCsv);
        Task<List<AccountGroupWithAccountsDto>> GetAccountGroupsWithAccountsAsync(int groupId = 0);
        Task<string> DeleteAccountAsync(int accountId, int userId);
        Task<string> AddOrDeleteAccountsAsync(AccountGroupActionRequest request);
    }
}