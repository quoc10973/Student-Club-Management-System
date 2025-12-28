using ClubManagementSystem.Repository.Entities;
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
    public interface IDepartmentService
    {
        Task<PagedResponse<object>> GetAllDepartments(DepartmentFilterRequest request, int page, int pageSize);
        Task<ApiResponse<DepartmentResponse>> GetDepartmentByIdAsync(int id);
        Task<ApiResponse<DepartmentResponse>> CreateAsync(DepartmentRequest request);
        Task<ApiResponse<DepartmentResponse>> UpdateAsync(int id, DepartmentRequest request);
        Task<ApiResponse<bool>> DeleteAsync(int id);
        Task<Deparment?> GetByCodeNameAsync(string codeName);
    }
}
