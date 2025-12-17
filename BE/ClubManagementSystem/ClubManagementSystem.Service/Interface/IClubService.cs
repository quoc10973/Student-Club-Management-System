using ClubManagementSystem.Service.Models.Common;
using ClubManagementSystem.Service.Models.Request;
using ClubManagementSystem.Service.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagementSystem.Service.Interface
{
    public interface IClubService
    {
        Task<PagedResponse<object>> GetAllClubs(ClubFilterRequest request, int page, int pageSize, int? accountId = null, string? role = null);
        Task<ApiResponse<ClubResponse>> GetClubByIdAsync(int id);
        Task<ApiResponse<ClubResponse>> CreateAsync(ClubRequest request);
        Task<ApiResponse<ClubResponse>> UpdateAsync(int id, ClubRequest request);
        Task<ApiResponse<bool>> DeleteAsync(int id);
        Task<ApiResponse<bool>> SetLeaderAsync(int clubId, int studentId, int adminAccountId);

    }
}
