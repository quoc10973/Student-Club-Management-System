using ClubManagementSystem.Repository.Entities;
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
    public interface IAuthenticationService
    {
        Task<ApiResponse<LoginResponse>> Login(LoginRequest loginRequest);
        Task<ApiResponse<LoginResponse>> LoginGoogle(string code, string redirectUri);
        Task<JwtTokenResult> GenerateToken(Account account);
    }
}
