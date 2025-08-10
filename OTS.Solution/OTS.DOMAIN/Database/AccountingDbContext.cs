

using Microsoft.EntityFrameworkCore;
using MobileAccounting.Entities;

namespace OTS.DOMAIN.Database
{
    public class AccountingDbContext : DbContext
    {
        public AccountingDbContext(DbContextOptions<AccountingDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<MobileUser> MobileUsers { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<AccountMaster> AccountMasters { get; set; }
        public DbSet<AccountGroupMaster> AccountGroupMasters { get; set; }
        public DbSet<AccountGroupDetail> AccountGroupDetails { get; set; }
        public DbSet<VoucherMaster> VoucherMasters { get; set; }
        public DbSet<VoucherMasterDeleted> VoucherMasterDeleteds { get; set; }
        public DbSet<VoucherDetail> VoucherDetails { get; set; }
        public DbSet<VoucherDetailsDeleted> VoucherDetailsDeleteds { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
      

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<AccountMaster>().ToTable("account_master");
        //}


    }
}
