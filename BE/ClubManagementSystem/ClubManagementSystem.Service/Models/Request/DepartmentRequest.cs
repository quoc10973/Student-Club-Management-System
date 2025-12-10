using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagementSystem.Service.Models.Request
{
    public class DepartmentRequest
    {
        [Required(ErrorMessage = "Code is required")]
        [MaxLength(10, ErrorMessage = "Code cannot exceed 10 characters")]
        public string? CodeName { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [MaxLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
        public string? Status { get; set; }
    }

    public class DepartmentFilterRequest
    {
        public int? Id { get; set; }
        public string? CodeName { get; set; }
        public string? Name { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }

        public string[]? SelectFields { get; set; }
    }
}
