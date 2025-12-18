using ClubManagementSystem.Service.Models.Common;
using ClubManagementSystem.Service.Models.Request;
using ClubManagementSystem.Service.Models.Response;
using System.Threading.Tasks;

namespace ClubManagementSystem.Service.Interface
{
    public interface IApplyRegistrationService
    {
        Task<ApiResponse<ApplyRegistrationResponse>> CreateAsync(ApplyRegistrationRequest request);
        Task<ApiResponse<ApplyRegistrationResponse>> GetApplyRegistrationByIdAsync(int id);
        Task<PagedResponse<object>> GetAllApplyRegistrations(ApplyRegistrationFilterRequest request, int page, int pageSize);
        Task<ApiResponse<ApplyRegistrationResponse>> ApproveAsync(int applyRegistrationId, int currentAccountId);
        Task<ApiResponse<ApplyRegistrationResponse>> RejectAsync(int applyRegistrationId, int currentAccountId);
    }

}












