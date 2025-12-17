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
    public interface IStudentService
    {
        Task<PagedResponse<object>> GetAllStudents(StudentFilterRequest request, int page, int pageSize);
        Task<ApiResponse<StudentResponse>> GetStudentByIdAsync(int id);
        Task<ApiResponse<StudentResponse>> CreateAsync(StudentRequest request);
        Task<ApiResponse<StudentResponse>> UpdateAsync(int id, StudentRequest request);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}








