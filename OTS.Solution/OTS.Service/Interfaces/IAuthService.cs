using System.Threading.Tasks;
using OTS.DOMAIN.MobileAccountingVM;

namespace OTS.Service.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseVM> LoginAsync(LoginRequestVM request);
    }
}
