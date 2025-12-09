using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagementSystem.Service.Models.Request
{
    public class StudentRequest
    {
        [Required(ErrorMessage = "AccountId is required")]
        public int AccountId { get; set; }

        [Required(ErrorMessage = "DeparmentId is required")]
        public int DeparmentId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [MaxLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
        public string? Status { get; set; }

        [Required(ErrorMessage = "Code is required")]
        [MaxLength(10, ErrorMessage = "Code cannot exceed 10 characters")]
        public string? Code { get; set; }
    }

    public class StudentFilterRequest
    {
        public int? Id { get; set; }
        public int? AccountId { get; set; }
        public int? DeparmentId { get; set; }
        public string? Status { get; set; }
        public string? Code { get; set; }
        public DateTime? CreatedAt { get; set; }

        public string[]? SelectFields { get; set; }
    }
}

