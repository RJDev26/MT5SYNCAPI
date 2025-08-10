using System;
using System.Threading.Tasks;

namespace MobileAccounting.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        //IUserRepository Users { get; }
        //IMobileUserRepository MobileUsers { get; }
        //IUserProfileRepository UserProfiles { get; }
        //IRoleRepository Roles { get; }
        //IPermissionRepository Permissions { get; }
        //IRolePermissionRepository RolePermissions { get; }
        //IUserRoleRepository UserRoles { get; }
        IAccountMasterRepository AccountMasters { get; }
        //IAccountGroupMasterRepository AccountGroupMasters { get; }
        //IAccountGroupDetailRepository AccountGroupDetails { get; }
        //IVoucherMasterRepository VoucherMasters { get; }
        //IVoucherMasterDeletedRepository VoucherMasterDeleteds { get; }
        //IVoucherDetailRepository VoucherDetails { get; }
        //IVoucherDetailsDeletedRepository VoucherDetailsDeleteds { get; }
        //IAuditLogRepository AuditLogs { get; }
        Task<int> CompleteAsync();
    }
}
