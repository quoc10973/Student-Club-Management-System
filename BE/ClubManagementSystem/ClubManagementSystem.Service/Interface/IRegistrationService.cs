using ClubManagementSystem.Service.Models.Common;
using ClubManagementSystem.Service.Models.Request;
using ClubManagementSystem.Service.Models.Response;
using System.Threading.Tasks;

namespace ClubManagementSystem.Service.Interface
{
    public interface IRegistrationService
    {
        Task<PagedResponse<object>> GetAllRegistrations(RegistrationFilterRequest request, int page, int pageSize);
        Task<ApiResponse<RegistrationResponse>> GetRegistrationByIdAsync(int id);
        Task<ApiResponse<RegistrationResponse>> CreateAsync(RegistrationRequest request);
        Task<ApiResponse<RegistrationResponse>> UpdateAsync(int id, RegistrationRequest request);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}









