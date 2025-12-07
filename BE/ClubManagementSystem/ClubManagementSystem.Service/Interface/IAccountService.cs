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
    public interface IAccountService
    {
        Task<PagedResponse<object>> GetAllAccounts(AccountFilterRequest request, int page, int pageSize);
        Task<ApiResponse<AccountResponse>> GetAccountByIdAsync(int id);
        Task<ApiResponse<AccountResponse>> CreateAsync(AccountRequest request);
        Task<ApiResponse<AccountResponse>> UpdateAsync(int id, AccountRequest request);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}
