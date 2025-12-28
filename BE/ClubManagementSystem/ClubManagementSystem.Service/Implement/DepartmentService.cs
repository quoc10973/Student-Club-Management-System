using ClubManagementSystem.Repository.Entities;
using ClubManagementSystem.Repository.Repositories;
using ClubManagementSystem.Service.Interface;
using ClubManagementSystem.Service.Models.Common;
using ClubManagementSystem.Service.Models.Request;
using ClubManagementSystem.Service.Models.Response;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagementSystem.Service.Implement
{
    public class DepartmentService : IDepartmentService
    {
        private readonly DepartmentRepository _departmentRepository;

        public DepartmentService(DepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<ApiResponse<DepartmentResponse>> CreateAsync(DepartmentRequest request)
        {
            // Check if CodeName already exists
            if (!string.IsNullOrEmpty(request.CodeName))
            {
                var existingDepartment = await GetByCodeNameAsync(request.CodeName);
                if (existingDepartment != null)
                {
                    return new ApiResponse<DepartmentResponse>
                    {
                        Success = false,
                        Message = "CodeName is existed.",
                        Data = null
                    };
                }
            }

            var entity = request.Adapt<Deparment>();
            entity.CreatedAt = DateTime.UtcNow;
            var affacted = await _departmentRepository.CreateAsync(entity);

            if (affacted > 0)
            {
                var response = entity.Adapt<DepartmentResponse>();
                return new ApiResponse<DepartmentResponse>
                {
                    Success = true,
                    Message = "Department created successfully.",
                    Data = response
                };
            }
            return new ApiResponse<DepartmentResponse>
            {
                Success = false,
                Message = "Failed to create department.",
                Data = null
            };
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var entity = await _departmentRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Department not found.",
                    Data = false
                };
            }

            var affacted = await _departmentRepository.RemoveAsync(entity);
            if (affacted)
            {
                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Department deleted successfully.",
                    Data = true
                };
            }
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Failed to delete department.",
                Data = false
            };
        }

        public async Task<PagedResponse<object>> GetAllDepartments(DepartmentFilterRequest request, int page, int pageSize)
        {
            var entity = request.Adapt<Deparment>();
            var query = _departmentRepository.GetFilteredDepartment(entity);

            IQueryable resultQuery = query;

            // Dynamic select fields
            if (request.SelectFields != null && request.SelectFields.Any())
            {
                var selectExpression = "new (" + string.Join(",", request.SelectFields) + ")";
                resultQuery = query.Select(selectExpression);
            }
            else
            {
                // Select full response object
                resultQuery = query.Select(d => new DepartmentResponse
                {
                    Id = d.Id,
                    CodeName = d.CodeName,
                    Name = d.Name,
                    Status = d.Status,
                    CreatedAt = d.CreatedAt
                });
            }

            // Count
            var totalCount = await ((IQueryable<dynamic>)resultQuery).CountAsync();

            // Paging
            var items = await resultQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToDynamicListAsync();

            return new PagedResponse<object>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<ApiResponse<DepartmentResponse>> GetDepartmentByIdAsync(int id)
        {
            var entity = await _departmentRepository.GetByIdAsync(id);
            if (entity != null)
            {
                var response = entity.Adapt<DepartmentResponse>();
                return new ApiResponse<DepartmentResponse>
                {
                    Success = true,
                    Message = "Department retrieved successfully.",
                    Data = response
                };
            }
            return new ApiResponse<DepartmentResponse>
            {
                Success = false,
                Message = "Department not found.",
                Data = null
            };
        }

        public async Task<ApiResponse<DepartmentResponse>> UpdateAsync(int id, DepartmentRequest request)
        {
            var entity = await _departmentRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return new ApiResponse<DepartmentResponse>
                {
                    Success = false,
                    Message = "Department not found.",
                    Data = null
                };
            }

            // Check if CodeName already exists for another department
            if (!string.IsNullOrEmpty(request.CodeName))
            {
                var existingDepartment = await GetByCodeNameAsync(request.CodeName);
                if (existingDepartment != null && existingDepartment.Id != id)
                {
                    return new ApiResponse<DepartmentResponse>
                    {
                        Success = false,
                        Message = "CodeName is existed.",
                        Data = null
                    };
                }
            }

            // Cập nhật các trường từ request
            entity.CodeName = request.CodeName;
            entity.Name = request.Name;
            entity.Status = request.Status;

            var affacted = await _departmentRepository.UpdateAsync(entity);
            if (affacted > 0)
            {
                var response = entity.Adapt<DepartmentResponse>();
                return new ApiResponse<DepartmentResponse>
                {
                    Success = true,
                    Message = "Department updated successfully.",
                    Data = response
                };
            }
            return new ApiResponse<DepartmentResponse>
            {
                Success = false,
                Message = "Failed to update department.",
                Data = null
            };
        }

        public async Task<Deparment?> GetByCodeNameAsync(string codeName)
        {
            return await _departmentRepository.GetByCodeNameAsync(codeName);
        }
    }
}
