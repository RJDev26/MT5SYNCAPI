using MobileAccounting.Entities;
using MobileAccounting.Repositories.Interfaces;
using OTS.DOMAIN.Database;

namespace MobileAccounting.Repositories.Implementations
{
    //public class UnitOfWork : IUnitOfWork

    //{
    //    private readonly AccountingDbContext _context;
    //    public IUserRepository Users { get; private set; }
    //    public IMobileUserRepository MobileUsers { get; private set; }
    //    public IUserProfileRepository UserProfiles { get; private set; }
    //    public IRoleRepository Roles { get; private set; }
    //    public IPermissionRepository Permissions { get; private set; }
    //    public IRolePermissionRepository RolePermissions { get; private set; }
    //    public IUserRoleRepository UserRoles { get; private set; }
    //    public IAccountMasterRepository AccountMasters { get; private set; }
    //    public IAccountGroupMasterRepository AccountGroupMasters { get; private set; }
    //    public IAccountGroupDetailRepository AccountGroupDetails { get; private set; }
    //    public IVoucherMasterRepository VoucherMasters { get; private set; }
    //    public IVoucherMasterDeletedRepository VoucherMasterDeleteds { get; private set; }
    //    public IVoucherDetailRepository VoucherDetails { get; private set; }
    //    public IVoucherDetailsDeletedRepository VoucherDetailsDeleteds { get; private set; }
    //    public IAuditLogRepository AuditLogs { get; private set; }

    //    public UnitOfWork(AccountingDbContext context, IAccountMasterRepository accountMasters, IUserRepository Users, IMobileUserRepository MobileUsers, IUserProfileRepository UserProfiles, IRoleRepository Roles, IPermissionRepository Permissions, IRolePermissionRepository RolePermissions, IUserRoleRepository UserRoles, IAccountMasterRepository AccountMasters, IAccountGroupMasterRepository AccountGroupMasters, IAccountGroupDetailRepository AccountGroupDetails, IVoucherMasterRepository VoucherMasters, IVoucherMasterDeletedRepository VoucherMasterDeleteds, IVoucherDetailRepository VoucherDetails, IVoucherDetailsDeletedRepository VoucherDetailsDeleteds, IAuditLogRepository AuditLogs)
    //    {
    //        _context = context;
    //        AccountMasters = accountMasters;
    //        Users = Users;
    //        MobileUsers = MobileUsers;
    //        UserProfiles = UserProfiles;
    //        Roles = Roles;
    //        Permissions = Permissions;
    //        RolePermissions = RolePermissions;
    //        UserRoles = UserRoles;
    //        AccountMasters = AccountMasters;
    //        AccountGroupMasters = AccountGroupMasters;
    //        AccountGroupDetails = AccountGroupDetails;
    //        VoucherMasters = VoucherMasters;
    //        VoucherMasterDeleteds = VoucherMasterDeleteds;
    //        VoucherDetails = VoucherDetails;
    //        VoucherDetailsDeleteds = VoucherDetailsDeleteds;
    //        AuditLogs = AuditLogs;
    //    }

    //    public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

    //    public void Dispose() => _context.Dispose();
    //}

    public class UnitOfWork : IUnitOfWork
    {
        private readonly AccountingDbContext _context;

        public IAccountMasterRepository AccountMasters { get; }

        public UnitOfWork(AccountingDbContext context, IAccountMasterRepository accountMasters)
        {
            _context = context;
            AccountMasters = accountMasters;
        }

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }

}
