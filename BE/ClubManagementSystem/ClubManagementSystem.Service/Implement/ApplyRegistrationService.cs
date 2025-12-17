using ClubManagementSystem.Repository.Entities;
using ClubManagementSystem.Repository.Repositories;
using ClubManagementSystem.Repository.DBContext;
using ClubManagementSystem.Service.Interface;
using ClubManagementSystem.Service.Models.Common;
using ClubManagementSystem.Service.Models.Request;
using ClubManagementSystem.Service.Models.Response;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Dynamic.Core;

namespace ClubManagementSystem.Service.Implement
{
    public class ApplyRegistrationService : IApplyRegistrationService
    {
        private readonly ApplyRegistrationRepository _applyRegistrationRepository;
        private readonly ClubMembershipRepository _clubMembershipRepository; 
        private readonly StudentRepository _studentRepository;
        private readonly RegistrationRepository _registrationRepository;
        private readonly ClubManagementSystemContext _context;

        public ApplyRegistrationService(
            ApplyRegistrationRepository applyRegistrationRepository,
            ClubMembershipRepository clubMemberRepository,
            StudentRepository studentRepository,
            RegistrationRepository registrationRepository,
            ClubManagementSystemContext context)
        {
            _applyRegistrationRepository = applyRegistrationRepository;
            _clubMembershipRepository = clubMemberRepository;
            _studentRepository = studentRepository;
            _registrationRepository = registrationRepository;
            _context = context;
        }

        public async Task<ApiResponse<ApplyRegistrationResponse>> CreateAsync(ApplyRegistrationRequest request)
        {
            // 1. Kiểm tra Registration tồn tại và có status là "Open"
            var registration = await _registrationRepository.GetByIdAsync(request.RegistrationId);
            if (registration == null)
            {
                return new ApiResponse<ApplyRegistrationResponse>
                {
                    Success = false,
                    Message = "Registration not found.",
                    Data = null
                };
            }

            if (registration.Status != "Open")
            {
                return new ApiResponse<ApplyRegistrationResponse>
                {
                    Success = false,
                    Message = $"Cannot apply to this registration. Registration status is '{registration.Status}'. Only 'Open' registrations allow applications.",
                    Data = null
                };
            }

            // 2. Kiểm tra Student tồn tại
            var student = await _studentRepository.GetByIdAsync(request.StudentId);
            if (student == null)
            {
                return new ApiResponse<ApplyRegistrationResponse>
                {
                    Success = false,
                    Message = "Student not found.",
                    Data = null
                };
            }

            // 3. Kiểm tra xem student đã apply vào registration này chưa
            var existingApply = await _applyRegistrationRepository.FirstOrDefaultAsync(a =>
                a.StudentId == request.StudentId &&
                a.RegistrationId == request.RegistrationId);

            if (existingApply != null)
            {
                return new ApiResponse<ApplyRegistrationResponse>
                {
                    Success = false,
                    Message = "You have already applied to this registration.",
                    Data = null
                };
            }

            // 4. Tạo ApplyRegistration với status mặc định là "Pending"
            var entity = request.Adapt<ApplyRegistration>();
            entity.Status = "Pending"; // Mặc định là Pending
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

            // Kiểm tra status hiện tại - không cho approve lại nếu đã approved/rejected
            if (apply.Status == "Approved")
            {
                return new ApiResponse<ApplyRegistrationResponse>
                {
                    Success = false,
                    Message = "This registration has already been approved.",
                    Data = null
                };
            }

            if (apply.Status == "Rejected")
            {
                return new ApiResponse<ApplyRegistrationResponse>
                {
                    Success = false,
                    Message = "This registration has been rejected and cannot be approved.",
                    Data = null
                };
            }

            // Club mà student đang apply vào
            var clubId = apply.Registration.ClubId;
            var studentId = apply.StudentId;

            // Lấy student tương ứng với account đang đăng nhập (Leader)
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

            // Kiểm tra xem student đã có membership trong club chưa
            var existingMembership = await _clubMembershipRepository.FirstOrDefaultAsync(cm =>
                cm.StudentId == studentId &&
                cm.ClubId == clubId);

            // Sử dụng transaction để đảm bảo tính nhất quán
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Update ApplyRegistration status
                apply.Status = "Approved";
                var affected = await _applyRegistrationRepository.UpdateAsync(apply);
                if (affected <= 0)
                {
                    await transaction.RollbackAsync();
                    return new ApiResponse<ApplyRegistrationResponse>
                    {
                        Success = false,
                        Message = "Failed to approve ApplyRegistration.",
                        Data = null
                    };
                }

                // 2. Tạo hoặc cập nhật ClubMembership
                if (existingMembership == null)
                {
                    // Tạo mới ClubMembership với role "Member"
                    var newMembership = new ClubMembership
                    {
                        StudentId = studentId,
                        ClubId = clubId,
                        ClubRole = "Member",
                        Status = "Active",
                        RequestedAt = DateTime.UtcNow,
                        ApprovedAt = DateTime.UtcNow,
                        ApprovedBy = currentAccountId,
                        Note = "Approved registration application"
                    };

                    var created = await _clubMembershipRepository.CreateAsync(newMembership);
                    if (created <= 0)
                    {
                        await transaction.RollbackAsync();
                        return new ApiResponse<ApplyRegistrationResponse>
                        {
                            Success = false,
                            Message = "Failed to create ClubMembership.",
                            Data = null
                        };
                    }
                }
                else
                {
                    // Nếu đã có membership nhưng đang InActive, update thành Active
                    // Nếu đã là Leader, giữ nguyên role
                    if (existingMembership.Status == "InActive")
                    {
                        existingMembership.Status = "Active";
                        existingMembership.ApprovedAt = DateTime.UtcNow;
                        existingMembership.ApprovedBy = currentAccountId;
                        existingMembership.Note = "Re-activated after approved registration";
                        
                        var updated = await _clubMembershipRepository.UpdateAsync(existingMembership);
                        if (updated <= 0)
                        {
                            await transaction.RollbackAsync();
                            return new ApiResponse<ApplyRegistrationResponse>
                            {
                                Success = false,
                                Message = "Failed to update ClubMembership.",
                                Data = null
                            };
                        }
                    }
                    // Nếu đã Active và là Member, không cần làm gì
                    // Nếu đã Active và là Leader, không cần làm gì
                }

                await transaction.CommitAsync();

                var response = apply.Adapt<ApplyRegistrationResponse>();
                return new ApiResponse<ApplyRegistrationResponse>
                {
                    Success = true,
                    Message = "ApplyRegistration approved successfully. Student has been added to the club as Member.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ApiResponse<ApplyRegistrationResponse>
                {
                    Success = false,
                    Message = $"Error approving registration: {ex.Message}",
                    Data = null
                };
            }
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


