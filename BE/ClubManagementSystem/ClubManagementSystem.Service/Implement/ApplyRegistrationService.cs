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
        private readonly ClubMembershipRepository _clubMembershipRepository; 
        private readonly StudentRepository _studentRepository;

        public ApplyRegistrationService(
            ApplyRegistrationRepository applyRegistrationRepository,
            ClubMembershipRepository clubMemberRepository,
            StudentRepository studentRepository)
        {
            _applyRegistrationRepository = applyRegistrationRepository;
            _clubMembershipRepository = clubMemberRepository;
            _studentRepository = studentRepository;
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

        public async Task<ApiResponse<ApplyRegistrationResponse>> ApproveAsync(int applyRegistrationId, int currentAccountId)
        {
            // Lấy ApplyRegistration + Registration
            var apply = await _applyRegistrationRepository.GetByIdWithRegistrationAsync(applyRegistrationId);
            if (apply == null)
            {
                return new ApiResponse<ApplyRegistrationResponse>
                {
                    Success = false,
                    Message = "ApplyRegistration not found.",
                    Data = null
                };
            }

            // Club mà student đang apply vào
            var clubId = apply.Registration.ClubId;

            // Lấy student tương ứng với account đang đăng nhập
            var leaderStudent = await _studentRepository.GetByAccountIdAsync(currentAccountId);
            if (leaderStudent == null)
            {
                return new ApiResponse<ApplyRegistrationResponse>
                {
                    Success = false,
                    Message = "Your account is not linked to any student.",
                    Data = null
                };
            }

            // Kiểm tra có membership Leader trong đúng club không
            var isLeader = await _clubMembershipRepository.AnyAsync(cm =>
                cm.StudentId == leaderStudent.Id &&
                cm.ClubId == clubId &&
                cm.ClubRole == "Leader");

            if (!isLeader)
            {
                return new ApiResponse<ApplyRegistrationResponse>
                {
                    Success = false,
                    Message = "You do not have permission to approve this registration.",
                    Data = null
                };
            }

            apply.Status = "Approved";

            var affected = await _applyRegistrationRepository.UpdateAsync(apply);
            if (affected <= 0)
            {
                return new ApiResponse<ApplyRegistrationResponse>
                {
                    Success = false,
                    Message = "Failed to approve ApplyRegistration.",
                    Data = null
                };
            }

            var response = apply.Adapt<ApplyRegistrationResponse>();
            return new ApiResponse<ApplyRegistrationResponse>
            {
                Success = true,
                Message = "ApplyRegistration approved successfully.",
                Data = response
            };
        }

        public async Task<ApiResponse<ApplyRegistrationResponse>> RejectAsync(int applyRegistrationId, int currentAccountId)
        {
            var apply = await _applyRegistrationRepository.GetByIdWithRegistrationAsync(applyRegistrationId);
            if (apply == null)
            {
                return new ApiResponse<ApplyRegistrationResponse>
                {
                    Success = false,
                    Message = "ApplyRegistration not found.",
                    Data = null
                };
            }

            var clubId = apply.Registration.ClubId;

            var leaderStudent = await _studentRepository.GetByAccountIdAsync(currentAccountId);
            if (leaderStudent == null)
            {
                return new ApiResponse<ApplyRegistrationResponse>
                {
                    Success = false,
                    Message = "Your account is not linked to any student.",
                    Data = null
                };
            }

            var isLeader = await _clubMembershipRepository.AnyAsync(cm =>
                cm.StudentId == leaderStudent.Id &&
                cm.ClubId == clubId &&
                cm.ClubRole == "Leader");

            if (!isLeader)
            {
                return new ApiResponse<ApplyRegistrationResponse>
                {
                    Success = false,
                    Message = "You do not have permission to reject this registration.",
                    Data = null
                };
            }

            apply.Status = "Rejected";

            var affected = await _applyRegistrationRepository.UpdateAsync(apply);
            if (affected <= 0)
            {
                return new ApiResponse<ApplyRegistrationResponse>
                {
                    Success = false,
                    Message = "Failed to reject ApplyRegistration.",
                    Data = null
                };
            }

            var response = apply.Adapt<ApplyRegistrationResponse>();
            return new ApiResponse<ApplyRegistrationResponse>
            {
                Success = true,
                Message = "ApplyRegistration rejected successfully.",
                Data = response
            };
        }
    }
}


