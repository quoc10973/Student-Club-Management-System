using ClubManagementSystem.Service.Models.Common;
using ClubManagementSystem.Service.Models.Request;
using ClubManagementSystem.Service.Models.Response;
using System.Threading.Tasks;

namespace ClubManagementSystem.Service.Interface
{
    public interface IApplyRegistrationService
    {
        Task<PagedResponse<object>> GetAllApplyRegistrations(ApplyRegistrationFilterRequest request, int page, int pageSize);
        Task<ApiResponse<ApplyRegistrationResponse>> GetApplyRegistrationByIdAsync(int id);
        Task<ApiResponse<ApplyRegistrationResponse>> CreateAsync(ApplyRegistrationRequest request);
        Task<ApiResponse<ApplyRegistrationResponse>> UpdateAsync(int id, ApplyRegistrationRequest request);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}


