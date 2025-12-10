using ClubManagementSystem.Repository.Entities;
using ClubManagementSystem.Repository.Repositories;
using ClubManagementSystem.Service.Interface;
using ClubManagementSystem.Service.Models.Common;
using ClubManagementSystem.Service.Models.Request;
using ClubManagementSystem.Service.Models.Response;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace ClubManagementSystem.Service.Implement
{
    public class ApplyRegistrationService : IApplyRegistrationService
    {
        private readonly ApplyRegistrationRepository _applyRegistrationRepository;

        public ApplyRegistrationService(ApplyRegistrationRepository applyRegistrationRepository)
        {
            _applyRegistrationRepository = applyRegistrationRepository;
        }

        public async Task<ApiResponse<ApplyRegistrationResponse>> CreateAsync(ApplyRegistrationRequest request)
        {
            var entity = request.Adapt<ApplyRegistration>();
            entity.CreatedAt = DateTime.UtcNow;

            var affected = await _applyRegistrationRepository.CreateAsync(entity);
            if (affected > 0)
            {
                var response = entity.Adapt<ApplyRegistrationResponse>();
                return new ApiResponse<ApplyRegistrationResponse>
                {
                    Success = true,
                    Message = "ApplyRegistration created successfully.",
                    Data = response
                };
            }

            return new ApiResponse<ApplyRegistrationResponse>
            {
                Success = false,
                Message = "Failed to create ApplyRegistration.",
                Data = null
            };
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var entity = await _applyRegistrationRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "ApplyRegistration not found.",
                    Data = false
                };
            }

            var removed = await _applyRegistrationRepository.RemoveAsync(entity);
            if (removed)
            {
                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "ApplyRegistration deleted successfully.",
                    Data = true
                };
            }

            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Failed to delete ApplyRegistration.",
                Data = false
            };
        }

        public async Task<ApiResponse<ApplyRegistrationResponse>> GetApplyRegistrationByIdAsync(int id)
        {
            var entity = await _applyRegistrationRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return new ApiResponse<ApplyRegistrationResponse>
                {
                    Success = false,
                    Message = "ApplyRegistration not found.",
                    Data = null
                };
            }

            var response = entity.Adapt<ApplyRegistrationResponse>();
            return new ApiResponse<ApplyRegistrationResponse>
            {
                Success = true,
                Message = "ApplyRegistration retrieved successfully.",
                Data = response
            };
        }

        public async Task<PagedResponse<object>> GetAllApplyRegistrations(ApplyRegistrationFilterRequest request, int page, int pageSize)
        {
            var entity = request.Adapt<ApplyRegistration>();
            var query = _applyRegistrationRepository.GetFilteredApplyRegistration(entity);

            IQueryable resultQuery = query;

            if (request.SelectFields != null && request.SelectFields.Any())
            {
                var selectExpression = "new (" + string.Join(",", request.SelectFields) + ")";
                resultQuery = query.Select(selectExpression);
            }
            else
            {
                resultQuery = query.Select(a => new ApplyRegistrationResponse
                {
                    Id = a.Id,
                    StudentId = a.StudentId,
                    RegistrationId = a.RegistrationId,
                    Status = a.Status,
                    CreatedAt = a.CreatedAt
                });
            }

            var totalCount = await ((IQueryable<dynamic>)resultQuery).CountAsync();
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

        public async Task<ApiResponse<ApplyRegistrationResponse>> UpdateAsync(int id, ApplyRegistrationRequest request)
        {
            var entity = await _applyRegistrationRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return new ApiResponse<ApplyRegistrationResponse>
                {
                    Success = false,
                    Message = "ApplyRegistration not found.",
                    Data = null
                };
            }

            entity.StudentId = request.StudentId;
            entity.RegistrationId = request.RegistrationId;
            entity.Status = request.Status;

            var affected = await _applyRegistrationRepository.UpdateAsync(entity);
            if (affected > 0)
            {
                var response = entity.Adapt<ApplyRegistrationResponse>();
                return new ApiResponse<ApplyRegistrationResponse>
                {
                    Success = true,
                    Message = "ApplyRegistration updated successfully.",
                    Data = response
                };
            }

            return new ApiResponse<ApplyRegistrationResponse>
            {
                Success = false,
                Message = "Failed to update ApplyRegistration.",
                Data = null
            };
        }
    }
}


