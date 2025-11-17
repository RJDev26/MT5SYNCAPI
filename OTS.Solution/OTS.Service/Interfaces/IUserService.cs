using System.Collections.Generic;
using System.Threading.Tasks;
using OTS.DOMAIN.MobileAccountingVM;

namespace OTS.Service.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseVM> CreateUserAsync(CreateUserRequestVM request);
        Task<UserResponseVM> UpdateUserAsync(string id, EditUserRequestVM request);
        Task<UserResponseVM> ChangePasswordAsync(string id, ChangePasswordRequestVM request);
        Task<UserResponseVM> ResetPasswordAsync(string id, ResetPasswordRequestVM request);
        Task<IEnumerable<UserListItemVM>> GetUserListAsync(string? role, bool? isActive);
        Task<UserResponseVM> SetUserManagerMappingAsync(UserManagerMappingRequestVM request);
        Task<IEnumerable<UserManagerVM>> GetManagersByUserIdAsync(int userId);
    }
}
