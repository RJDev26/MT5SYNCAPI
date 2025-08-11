using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MobileAccounting.Repositories.Implementations;
using MobileAccounting.Repositories.Interfaces;
using MobileAccounting.Services;
using MobileAccounting.Services.Interfaces;
using OTS.Infrastructutre.Generic.WebBroker.DataAccessCore;
using OTS.Service.Interfaces;

namespace OTS.Service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<DbManager>(provider =>
            {
                var config = provider.GetRequiredService<IConfiguration>();
                var connString = config.GetConnectionString("DefaultConnection");
                return new DbManager(connString);
            });
            //var connectionString = configuration.GetConnectionString("DefaultConnection");
            //services.AddDbContext<AccountingDbContext>(options =>
            //    options.UseSqlServer(connectionString));

            // Register repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IMobileUserRepository, MobileUserRepository>();
            services.AddScoped<IUserProfileRepository, UserProfileRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IAccountMasterRepository, AccountMasterRepository>();
            services.AddScoped<IAccountGroupMasterRepository, AccountGroupMasterRepository>();
            services.AddScoped<IAccountGroupDetailRepository, AccountGroupDetailRepository>();
            services.AddScoped<IVoucherMasterRepository, VoucherMasterRepository>();
            services.AddScoped<IVoucherMasterDeletedRepository, VoucherMasterDeletedRepository>();
            services.AddScoped<IVoucherDetailRepository, VoucherDetailRepository>();
            services.AddScoped<IVoucherDetailsDeletedRepository, VoucherDetailsDeletedRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<ILiveDealRepository, LiveDealRepository>();
            services.AddScoped<IOrderSnapshotRepository, OrderSnapshotRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register services
            services.AddScoped<IAccountMasterService, AccountMasterService>();
            services.AddScoped<IVoucherService, VoucherService>();
            services.AddScoped<ILiveDealService, LiveDealService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IOrderSnapshotService, OrderSnapshotService>();

            return services;
        }
    }

}
