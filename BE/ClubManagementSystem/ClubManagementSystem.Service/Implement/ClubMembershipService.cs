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
    public class ClubMembershipService : IClubMembershipService
    {
        private readonly ClubMembershipRepository _clubMembershipRepository;

        public ClubMembershipService(ClubMembershipRepository clubMembershipRepository)
        {
            _clubMembershipRepository = clubMembershipRepository;
        }

        public async Task<ApiResponse<ClubMembershipResponse>> CreateAsync(ClubMembershipRequest request)
        {
            var entity = request.Adapt<ClubMembership>();
            entity.RequestedAt = DateTime.UtcNow;

            var affected = await _clubMembershipRepository.CreateAsync(entity);
            if (affected > 0)
            {
                var response = entity.Adapt<ClubMembershipResponse>();
                return new ApiResponse<ClubMembershipResponse>
                {
                    Success = true,
                    Message = "ClubMembership created successfully.",
                    Data = response
                };
            }

            return new ApiResponse<ClubMembershipResponse>
            {
                Success = false,
                Message = "Failed to create ClubMembership.",
                Data = null
            };
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var entity = await _clubMembershipRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "ClubMembership not found.",
                    Data = false
                };
            }

            var removed = await _clubMembershipRepository.RemoveAsync(entity);
            if (removed)
            {
                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "ClubMembership deleted successfully.",
                    Data = true
                };
            }

            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Failed to delete ClubMembership.",
                Data = false
            };
        }

        public async Task<ApiResponse<ClubMembershipResponse>> GetClubMembershipByIdAsync(int id)
        {
            var entity = await _clubMembershipRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return new ApiResponse<ClubMembershipResponse>
                {
                    Success = false,
                    Message = "ClubMembership not found.",
                    Data = null
                };
            }

            var response = entity.Adapt<ClubMembershipResponse>();
            return new ApiResponse<ClubMembershipResponse>
            {
                Success = true,
                Message = "ClubMembership retrieved successfully.",
                Data = response
            };
        }

        public async Task<PagedResponse<object>> GetAllClubMemberships(ClubMembershipFilterRequest request, int page, int pageSize)
        {
            var entity = request.Adapt<ClubMembership>();
            var query = _clubMembershipRepository.GetFilteredClubMembership(entity);

            IQueryable resultQuery = query;

            if (request.SelectFields != null && request.SelectFields.Any())
            {
                var selectExpression = "new (" + string.Join(",", request.SelectFields) + ")";
                resultQuery = query.Select(selectExpression);
            }
            else
            {
                resultQuery = query.Select(m => new ClubMembershipResponse
                {
                    Id = m.Id,
                    StudentId = m.StudentId,
                    ClubId = m.ClubId,
                    ClubRole = m.ClubRole,
                    Status = m.Status,
                    RequestedAt = m.RequestedAt,
                    ApprovedAt = m.ApprovedAt,
                    ApprovedBy = m.ApprovedBy,
                    Note = m.Note
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

        public async Task<ApiResponse<ClubMembershipResponse>> UpdateAsync(int id, ClubMembershipRequest request)
        {
            var entity = await _clubMembershipRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return new ApiResponse<ClubMembershipResponse>
                {
                    Success = false,
                    Message = "ClubMembership not found.",
                    Data = null
                };
            }

            entity.StudentId = request.StudentId;
            entity.ClubId = request.ClubId;
            entity.ClubRole = request.ClubRole;
            entity.Status = request.Status;
            entity.Note = request.Note;

            var affected = await _clubMembershipRepository.UpdateAsync(entity);
            if (affected > 0)
            {
                var response = entity.Adapt<ClubMembershipResponse>();
                return new ApiResponse<ClubMembershipResponse>
                {
                    Success = true,
                    Message = "ClubMembership updated successfully.",
                    Data = response
                };
            }

            return new ApiResponse<ClubMembershipResponse>
            {
                Success = false,
                Message = "Failed to update ClubMembership.",
                Data = null
            };
        }
    }
}


