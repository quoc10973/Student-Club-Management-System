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
    public class AccountService : IAccountService
    {
        private readonly AccountRepository _accountRepository;
        public AccountService(AccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<ApiResponse<AccountResponse>> CreateAsync(AccountRequest request)
        {
            var entity = request.Adapt<Account>();
            entity.PasswordHash = HashPassword(request.Password);
            entity.CreatedAt = DateTime.UtcNow;
            var affacted = await _accountRepository.CreateAsync(entity);
            if (affacted > 0)
            {
                var response = entity.Adapt<AccountResponse>();
                return new ApiResponse<AccountResponse>
                {
                    Success = true,
                    Message = "Account created successfully.",
                    Data = response
                };
            }
            else
            {
                return new ApiResponse<AccountResponse>
                {
                    Success = false,
                    Message = "Failed to create account.",
                    Data = null
                };
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var entity = await _accountRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Account not found.",
                    Data = false
                };
            }
            var affacted = await _accountRepository.RemoveAsync(entity);
            if (affacted)
            {
                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Account deleted successfully.",
                    Data = true
                };
            }
            else
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Failed to delete account.",
                    Data = false
                };
            }
        }

        public async Task<ApiResponse<AccountResponse>> GetAccountByIdAsync(int id)
        {
            var entity = await _accountRepository.GetByIdAsync(id);
            if (entity != null)
            {
                var response = entity.Adapt<AccountResponse>();
                return new ApiResponse<AccountResponse>
                {
                    Success = true,
                    Message = "Account retrieved successfully.",
                    Data = response
                };
            }
            else
            {
                return new ApiResponse<AccountResponse>
                {
                    Success = false,
                    Message = "Account not found.",
                    Data = null
                };
            }
        }

        public async Task<PagedResponse<object>> GetAllAccounts(AccountFilterRequest request, int page, int pageSize)
        {
            var entity = request.Adapt<Account>();
            var query = _accountRepository.GetFilteredAccount(entity);

            IQueryable resultQuery = query;

            // Nếu client có truyền SelectFields → dynamic select
            if (request.SelectFields != null && request.SelectFields.Any())
            {
                var selectExpression = "new (" + string.Join(",", request.SelectFields) + ")";
                resultQuery = query.Select(selectExpression);    // 🔥 Dynamic LINQ trả về anonymous type
            }
            else
            {
                // Không truyền SelectFields → lấy full AccountResponse
                resultQuery = query.Select(a => new AccountResponse
                {
                    Id = a.Id,
                    Username = a.Username,
                    FullName = a.FullName,
                    Email = a.Email,
                    Phone = a.Phone,
                    Role = a.Role,
                    Status = a.Status,
                    CreatedAt = a.CreatedAt
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

        public async Task<ApiResponse<AccountResponse>> UpdateAsync(int id, AccountRequest request)
        {
            var entity = await _accountRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return new ApiResponse<AccountResponse>
                {
                    Success = false,
                    Message = "Account not found.",
                    Data = null
                };
            }
            entity.PasswordHash = HashPassword(request.Password);
            entity.FullName = request.FullName;
            entity.Email = request.Email;
            entity.Phone = request.Phone;
            entity.Role = request.Role;
            entity.Status = request.Status;
            var affacted = await _accountRepository.UpdateAsync(entity);
            if (affacted > 0)
            {
                var response = entity.Adapt<AccountResponse>();
                return new ApiResponse<AccountResponse>
                {
                    Success = true,
                    Message = "Account updated successfully.",
                    Data = response
                };
            }
            else
            {
                return new ApiResponse<AccountResponse>
                {
                    Success = false,
                    Message = "Failed to update account.",
                    Data = null
                };
            }
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
