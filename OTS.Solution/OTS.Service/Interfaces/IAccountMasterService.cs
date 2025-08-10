using Microsoft.Identity.Client;
using MobileAccounting.Entities;
using OTS.DOMAIN.MobileAccountingVM;
using OTS.DOMAIN.MobileAccoutingDTO;

namespace MobileAccounting.Services.Interfaces
{
    public interface IAccountMasterService
    {
     
        Task<string> SaveOrUpdateAsync(AccountMaster model);
        Task<List<AccountMaster>> GetAccountsAsync(int? accountId = null);
        Task<(int GroupId, string Message)> SaveOrUpdateGroupAsync(AccountGroupRequest request);
        Task<List<AccountGroupVM>> GetAccountGroupsAsync();
        Task<List<AccountGroupMappingDto>> GetMappingsAsync();
        Task<bool> AddAccountsToGroupAsync(AddAccountsToGroupRequest request);
        Task<List<AccountGroupWithAccountsDto>> GetAccountGroupsWithAccountsAsync(int groupId = 0);
        Task<string> DeleteAccountAsync(DeleteAccountRequest request);
        Task<string> AddOrDeleteAccountsAsync(AccountGroupActionRequest request);
    }
}
