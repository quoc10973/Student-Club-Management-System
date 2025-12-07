using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagementSystem.Service.Models.Response
{
    public class AccountResponse
    {
        public int Id { get; set; }
        public string Username { get; set; } 
        public string Email { get; set; } 
        public string FullName { get; set; } 
        public string Phone { get; set; } 
        public string Role { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
