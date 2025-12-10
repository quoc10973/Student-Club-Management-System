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
    public class RegistrationService : IRegistrationService
    {
        private readonly RegistrationRepository _registrationRepository;

        public RegistrationService(RegistrationRepository registrationRepository)
        {
            _registrationRepository = registrationRepository;
        }

        public async Task<ApiResponse<RegistrationResponse>> CreateAsync(RegistrationRequest request)
        {
            if (request.StartDay.HasValue && request.EndDay.HasValue && request.StartDay > request.EndDay)
            {
                return new ApiResponse<RegistrationResponse>
                {
                    Success = false,
                    Message = "StartDay must be before EndDay.",
                    Data = null
                };
            }

            var entity = request.Adapt<Registration>();
            entity.CreatedAt = DateTime.UtcNow;

            var affected = await _registrationRepository.CreateAsync(entity);
            if (affected > 0)
            {
                var response = entity.Adapt<RegistrationResponse>();
                return new ApiResponse<RegistrationResponse>
                {
                    Success = true,
                    Message = "Registration created successfully.",
                    Data = response
                };
            }

            return new ApiResponse<RegistrationResponse>
            {
                Success = false,
                Message = "Failed to create registration.",
                Data = null
            };
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var entity = await _registrationRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Registration not found.",
                    Data = false
                };
            }

            var hasPending = await _registrationRepository.HasPendingApplyRegistrationsAsync(id);
            if (hasPending)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Cannot delete registration while there are pending apply registrations.",
                    Data = false
                };
            }

            // Xóa tất cả ApplyRegistration (Approved/Rejected/Pending đã được chặn ở trên)
            await _registrationRepository.DeleteApplyRegistrationsAsync(id);

            var removed = await _registrationRepository.RemoveAsync(entity);
            if (removed)
            {
                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Registration deleted successfully.",
                    Data = true
                };
            }

            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Failed to delete registration.",
                Data = false
            };
        }

        public async Task<ApiResponse<RegistrationResponse>> GetRegistrationByIdAsync(int id)
        {
            var entity = await _registrationRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return new ApiResponse<RegistrationResponse>
                {
                    Success = false,
                    Message = "Registration not found.",
                    Data = null
                };
            }

            var response = entity.Adapt<RegistrationResponse>();
            return new ApiResponse<RegistrationResponse>
            {
                Success = true,
                Message = "Registration retrieved successfully.",
                Data = response
            };
        }

        public async Task<PagedResponse<object>> GetAllRegistrations(RegistrationFilterRequest request, int page, int pageSize)
        {
            var entity = request.Adapt<Registration>();
            var query = _registrationRepository.GetFilteredRegistration(entity);

            IQueryable resultQuery = query;

            if (request.SelectFields != null && request.SelectFields.Any())
            {
                var selectExpression = "new (" + string.Join(",", request.SelectFields) + ")";
                resultQuery = query.Select(selectExpression);
            }
            else
            {
                resultQuery = query.Select(r => new RegistrationResponse
                {
                    Id = r.Id,
                    ClubId = r.ClubId,
                    StartDay = r.StartDay,
                    EndDay = r.EndDay,
                    Status = r.Status,
                    Note = r.Note,
                    CreatedAt = r.CreatedAt
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

        public async Task<ApiResponse<RegistrationResponse>> UpdateAsync(int id, RegistrationRequest request)
        {
            var entity = await _registrationRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return new ApiResponse<RegistrationResponse>
                {
                    Success = false,
                    Message = "Registration not found.",
                    Data = null
                };
            }

            if (request.StartDay.HasValue && request.EndDay.HasValue && request.StartDay > request.EndDay)
            {
                return new ApiResponse<RegistrationResponse>
                {
                    Success = false,
                    Message = "StartDay must be before EndDay.",
                    Data = null
                };
            }

            entity.ClubId = request.ClubId;
            entity.StartDay = request.StartDay;
            entity.EndDay = request.EndDay;
            entity.Status = request.Status;
            entity.Note = request.Note;

            var affected = await _registrationRepository.UpdateAsync(entity);
            if (affected > 0)
            {
                var response = entity.Adapt<RegistrationResponse>();
                return new ApiResponse<RegistrationResponse>
                {
                    Success = true,
                    Message = "Registration updated successfully.",
                    Data = response
                };
            }

            return new ApiResponse<RegistrationResponse>
            {
                Success = false,
                Message = "Failed to update registration.",
                Data = null
            };
        }
    }
}

