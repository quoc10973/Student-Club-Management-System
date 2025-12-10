using ClubManagementSystem.Service.Models.Common;
using ClubManagementSystem.Service.Models.Request;
using ClubManagementSystem.Service.Models.Response;
using System.Threading.Tasks;

namespace ClubManagementSystem.Service.Interface
{
    public interface IClubMembershipService
    {
        Task<PagedResponse<object>> GetAllClubMemberships(ClubMembershipFilterRequest request, int page, int pageSize);
        Task<ApiResponse<ClubMembershipResponse>> GetClubMembershipByIdAsync(int id);
        Task<ApiResponse<ClubMembershipResponse>> CreateAsync(ClubMembershipRequest request);
        Task<ApiResponse<ClubMembershipResponse>> UpdateAsync(int id, ClubMembershipRequest request);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}


