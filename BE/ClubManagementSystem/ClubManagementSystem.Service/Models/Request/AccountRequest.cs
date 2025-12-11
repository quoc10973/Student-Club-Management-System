using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagementSystem.Service.Models.Request
{
    public class AccountRequest
    {
        [Required(ErrorMessage = "Username is required")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "FullName is required")]
        [MaxLength(100, ErrorMessage = "FullName cannot exceed 100 characters")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone must start with 0 and contain exactly 10 digits")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [RegularExpression("^(User|Admin)$", ErrorMessage = "Role must be User or Admin")]
        public string? Role { get; set; }
        [Required(ErrorMessage = "Status is required")]
        [RegularExpression("^(Active|InActive|Deleted)$", ErrorMessage = "Status must be Active, InActive or Deleted")]
        public string? Status { get; set; }
    }

    public class AccountFilterRequest
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Role { get; set; }
        public string? Status { get; set; }
        public DateTime? CreateAt { get; set; }

        public string[]? SelectFields { get; set; }
    }
}
