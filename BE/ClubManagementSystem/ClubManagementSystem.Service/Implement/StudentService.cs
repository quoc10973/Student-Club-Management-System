using ClubManagementSystem.Repository.Entities;
using ClubManagementSystem.Repository.Repositories;
using ClubManagementSystem.Service.Interface;
using ClubManagementSystem.Service.Models.Common;
using ClubManagementSystem.Service.Models.Request;
using ClubManagementSystem.Service.Models.Response;
using Mapster;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagementSystem.Service.Implement
{
    public class StudentService : IStudentService
    {
        private readonly StudentRepository _studentRepository;
        public StudentService(StudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<ApiResponse<StudentResponse>> CreateAsync(StudentRequest request)
        {
            // Check if code already exists
            var existingStudent = await GetByCodeAsync(request.Code);
            if (existingStudent != null)
            {
                return new ApiResponse<StudentResponse>
                {
                    Success = false,
                    Message = "Code is existed.",
                    Data = null
                };
            }

            var entity = request.Adapt<Student>();
            entity.CreatedAt = DateTime.UtcNow;
            var affacted = await _studentRepository.CreateAsync(entity);
            if (affacted > 0)
            {
                var response = entity.Adapt<StudentResponse>();
                return new ApiResponse<StudentResponse>
                {
                    Success = true,
                    Message = "Student created successfully.",
                    Data = response
                };
            }
            else
            {
                return new ApiResponse<StudentResponse>
                {
                    Success = false,
                    Message = "Failed to create student.",
                    Data = null
                };
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var entity = await _studentRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Student not found.",
                    Data = false
                };
            }
            var affacted = await _studentRepository.RemoveAsync(entity);
            if (affacted)
            {
                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Student deleted successfully.",
                    Data = true
                };
            }
            else
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Failed to delete student.",
                    Data = false
                };
            }
        }

        public async Task<ApiResponse<StudentResponse>> GetStudentByIdAsync(int id)
        {
            var entity = await _studentRepository.GetByIdAsync(id);
            if (entity != null)
            {
                var response = entity.Adapt<StudentResponse>();
                return new ApiResponse<StudentResponse>
                {
                    Success = true,
                    Message = "Student retrieved successfully.",
                    Data = response
                };
            }
            else
            {
                return new ApiResponse<StudentResponse>
                {
                    Success = false,
                    Message = "Student not found.",
                    Data = null
                };
            }
        }

        public async Task<PagedResponse<object>> GetAllStudents(StudentFilterRequest request, int page, int pageSize)
        {
            var entity = request.Adapt<Student>();
            var query = _studentRepository.GetFilteredStudent(entity);

            IQueryable resultQuery = query;

            // Nếu client có truyền SelectFields → dynamic select
            if (request.SelectFields != null && request.SelectFields.Any())
            {
                var selectExpression = "new (" + string.Join(",", request.SelectFields) + ")";
                resultQuery = query.Select(selectExpression);    // 🔥 Dynamic LINQ trả về anonymous type
            }
            else
            {
                // Không truyền SelectFields → lấy full StudentResponse
                resultQuery = query.Select(s => new StudentResponse
                {
                    Id = s.Id,
                    AccountId = s.AccountId,
                    DeparmentId = s.DeparmentId,
                    Status = s.Status,
                    Code = s.Code,
                    CreatedAt = s.CreatedAt
                });
            }

            // Count
            var totalCount = await ((IQueryable<dynamic>)resultQuery).CountAsync();

            // Paging
            var items = await resultQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToDynamicListAsync();   // 🔥 Quan trọng: dynamic list

            return new PagedResponse<object>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<ApiResponse<StudentResponse>> UpdateAsync(int id, StudentRequest request)
        {
            var entity = await _studentRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return new ApiResponse<StudentResponse>
                {
                    Success = false,
                    Message = "Student not found.",
                    Data = null
                };
            }

            // Check if code already exists for another student
            var existingStudent = await GetByCodeAsync(request.Code);
            if (existingStudent != null && existingStudent.Id != id)
            {
                return new ApiResponse<StudentResponse>
                {
                    Success = false,
                    Message = "Code is existed.",
                    Data = null
                };
            }

            entity.AccountId = request.AccountId;
            entity.DeparmentId = request.DeparmentId;
            entity.Status = request.Status;
            entity.Code = request.Code;
            var affacted = await _studentRepository.UpdateAsync(entity);
            if (affacted > 0)
            {
                var response = entity.Adapt<StudentResponse>();
                return new ApiResponse<StudentResponse>
                {
                    Success = true,
                    Message = "Student updated successfully.",
                    Data = response
                };
            }
            else
            {
                return new ApiResponse<StudentResponse>
                {
                    Success = false,
                    Message = "Failed to update student.",
                    Data = null
                };
            }
        }

        public async Task<Student?> GetByCodeAsync(string code)
        {
            return await _studentRepository.GetByCodeAsync(code);
        }
    }
}

