using Microsoft.Identity.Client;
using MobileAccounting.Entities;
using MobileAccounting.Repositories.Interfaces;
using MobileAccounting.Services.Interfaces;
using OTS.DOMAIN.MobileAccountingVM;
using OTS.DOMAIN.MobileAccoutingDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileAccounting.Services
{
    public class AccountMasterService : IAccountMasterService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountMasterService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        
        public async Task<string> SaveOrUpdateAsync(AccountMaster model)
        {
            return await _unitOfWork.AccountMasters.SaveOrUpdateAccountAsync(model);
        }
        public async Task<List<AccountMaster>> GetAccountsAsync(int? accountId = null)
        {
            return await _unitOfWork.AccountMasters.GetAccountsAsync(accountId);
        }
       

      
        public async Task<(int GroupId, string Message)> SaveOrUpdateGroupAsync(AccountGroupRequest request)
        {
            var message = await _unitOfWork.AccountMasters.SaveOrUpdateAccountGroupAsync(request);
            return (request.GroupId ?? 0, message);
        }
        public async Task<List<AccountGroupVM>> GetAccountGroupsAsync()
        {
            return await _unitOfWork.AccountMasters.GetAccountGroupListAsync();
        }
        public Task<List<AccountGroupMappingDto>> GetMappingsAsync()
        {
            return _unitOfWork.AccountMasters.GetAccountGroupMappingsAsync();
        }
        public async Task<bool> AddAccountsToGroupAsync(AddAccountsToGroupRequest request)
        {
            string csv = string.Join(",", request.AccountIds);
            return await _unitOfWork.AccountMasters.AddAccountsToGroupAsync(request.GroupId, csv);
        }
        public async Task<List<AccountGroupWithAccountsDto>> GetAccountGroupsWithAccountsAsync(int groupId = 0)
        {
            return await _unitOfWork.AccountMasters.GetAccountGroupsWithAccountsAsync(groupId);
        }
        public async Task<string> DeleteAccountAsync(DeleteAccountRequest request)
        {
            return await _unitOfWork.AccountMasters.DeleteAccountAsync(request.AccountId, request.UserId);
        }
        public async Task<string> AddOrDeleteAccountsAsync(AccountGroupActionRequest request)
        {
            return await _unitOfWork.AccountMasters.AddOrDeleteAccountsAsync(request);
        }
    }
}
