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
    public class ClubService : IClubService
    {
        private readonly ClubRepository _clubRepository;
        private readonly StudentRepository _studentRepository;
        private readonly ClubMembershipRepository _clubMembershipRepository;

        public ClubService(
            ClubRepository clubRepository,
            StudentRepository studentRepository,
            ClubMembershipRepository clubMembershipRepository)
        {
            _clubRepository = clubRepository;
            _studentRepository = studentRepository;
            _clubMembershipRepository = clubMembershipRepository;
        }

        public async Task<ApiResponse<ClubResponse>> CreateAsync(ClubRequest request)
        {
            var entity = request.Adapt<Club>();

            // Xử lý logic: Nếu IsPublic là true thì DeparmentId phải là null/0
            if (request.IsPublic == true)
            {
                entity.DeparmentId = null;
            }
            // Nếu IsPublic là false, phải có DeparmentId
            else if (request.IsPublic == false && (request.DeparmentId == null || request.DeparmentId == 0))
            {
                return new ApiResponse<ClubResponse>
                {
                    Success = false,
                    Message = "A private club (IsPublic=false) must be assigned to a DeparmentId.",
                    Data = null
                };
            }

            entity.CreatedAt = DateTime.UtcNow;
            var affected = await _clubRepository.CreateAsync(entity);

            if (affected > 0)
            {
                var response = entity.Adapt<ClubResponse>();
                return new ApiResponse<ClubResponse>
                {
                    Success = true,
                    Message = "Club created successfully.",
                    Data = response
                };
            }
            return new ApiResponse<ClubResponse>
            {
                Success = false,
                Message = "Failed to create club.",
                Data = null
            };
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var entity = await _clubRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Club not found.",
                    Data = false
                };
            }

            var affected = await _clubRepository.RemoveAsync(entity);
            if (affected)
            {
                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Club deleted successfully.",
                    Data = true
                };
            }
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Failed to delete club.",
                Data = false
            };
        }

        public async Task<PagedResponse<object>> GetAllClubs(ClubFilterRequest request, int page, int pageSize)
        {
            var entity = request.Adapt<Club>();

            // Sử dụng logic lọc tùy chỉnh trong Repository
            var query = _clubRepository.GetFilteredClub(
                entity,
                request.IsPublic,
                request.DeparmentId
            );

            IQueryable resultQuery = query;

            // Dynamic select fields (tương tự Student)
            if (request.SelectFields != null && request.SelectFields.Any())
            {
                var selectExpression = "new (" + string.Join(",", request.SelectFields) + ")";
                resultQuery = query.Select(selectExpression);
            }
            else
            {
                // Select full response object
                resultQuery = query.Select(c => new ClubResponse
                {
                    Id = c.Id,
                    DeparmentId = c.DeparmentId,
                    Name = c.Name,
                    IsPublic = c.IsPublic,
                    Description = c.Description,
                    EstablishedDate = c.EstablishedDate,
                    Status = c.Status,
                    CreatedAt = c.CreatedAt
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

        public async Task<ApiResponse<ClubResponse>> GetClubByIdAsync(int id)
        {
            var entity = await _clubRepository.GetByIdAsync(id);
            if (entity != null)
            {
                var response = entity.Adapt<ClubResponse>();
                return new ApiResponse<ClubResponse>
                {
                    Success = true,
                    Message = "Club retrieved successfully.",
                    Data = response
                };
            }
            return new ApiResponse<ClubResponse>
            {
                Success = false,
                Message = "Club not found.",
                Data = null
            };
        }

        public async Task<ApiResponse<ClubResponse>> UpdateAsync(int id, ClubRequest request)
        {
            var entity = await _clubRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return new ApiResponse<ClubResponse>
                {
                    Success = false,
                    Message = "Club not found.",
                    Data = null
                };
            }

            // Cập nhật các trường từ request
            entity.Name = request.Name;
            entity.IsPublic = request.IsPublic;
            entity.Description = request.Description;
            entity.EstablishedDate = request.EstablishedDate;
            entity.Status = request.Status;

            // Xử lý logic: Nếu IsPublic là true thì DeparmentId phải là null/0
            if (request.IsPublic == true)
            {
                entity.DeparmentId = null;
            }
            // Nếu IsPublic là false, phải có DeparmentId
            else if (request.IsPublic == false && (request.DeparmentId == null || request.DeparmentId == 0))
            {
                return new ApiResponse<ClubResponse>
                {
                    Success = false,
                    Message = "A private club (IsPublic=false) must be assigned to a DeparmentId.",
                    Data = null
                };
            }
            else
            {
                entity.DeparmentId = request.DeparmentId;
            }

            var affected = await _clubRepository.UpdateAsync(entity);
            if (affected > 0)
            {
                var response = entity.Adapt<ClubResponse>();
                return new ApiResponse<ClubResponse>
                {
                    Success = true,
                    Message = "Club updated successfully.",
                    Data = response
                };
            }
            return new ApiResponse<ClubResponse>
            {
                Success = false,
                Message = "Failed to update club.",
                Data = null
            };
        }
        public async Task<ApiResponse<bool>> SetLeaderAsync(int clubId, int studentId, int adminAccountId)
        {
            // 1. Kiểm tra club tồn tại
            var club = await _clubRepository.GetByIdAsync(clubId);
            if (club == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Club not found.",
                    Data = false
                };
            }

            // 2. Kiểm tra student tồn tại
            var student = await _studentRepository.GetByIdAsync(studentId);
            if (student == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Student not found.",
                    Data = false
                };
            }

            // 3. Tìm membership của chính student này trong club
            var membershipOfStudent = await _clubMembershipRepository
                .FirstOrDefaultAsync(cm => cm.ClubId == clubId && cm.StudentId == studentId);

            // 4. Nếu student chưa có membership trong club -> TẠO MỚI LEADER
            if (membershipOfStudent == null)
            {
                var newMembership = new ClubMembership
                {
                    StudentId = studentId,
                    ClubId = clubId,
                    ClubRole = "Leader",
                    Status = "Active",
                    RequestedAt = DateTime.UtcNow,
                    ApprovedAt = DateTime.UtcNow,
                    ApprovedBy = adminAccountId,
                    Note = "Leader assigned by admin"
                };

                var created = await _clubMembershipRepository.CreateAsync(newMembership);
                if (created <= 0)
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Failed to add leader membership.",
                        Data = false
                    };
                }
            }
            else
            {
                // 5. Student đã có membership trong club -> nâng lên Leader
                membershipOfStudent.ClubRole = "Leader";
                membershipOfStudent.Status = "Active";
                membershipOfStudent.ApprovedAt = DateTime.UtcNow;
                membershipOfStudent.ApprovedBy = adminAccountId;
                membershipOfStudent.Note = "Promoted to leader by admin";

                var updated = await _clubMembershipRepository.UpdateAsync(membershipOfStudent);
                if (updated <= 0)
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Failed to update leader membership.",
                        Data = false
                    };
                }
            }

            return new ApiResponse<bool>
            {
                Success = true,
                Message = "Leader added to membership successfully.",
                Data = true
            };
        }

    }
}
