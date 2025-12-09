using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagementSystem.Service.Models.Request
{
    public class ClubRequest
    {
        // Có thể null nếu IsPublic=true
        public int? DeparmentId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string? Name { get; set; }

        // Mặc định là Public nếu không truyền
        public bool? IsPublic { get; set; } = true;

        public string? Description { get; set; }

        public DateOnly? EstablishedDate { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [MaxLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
        public string? Status { get; set; }
    }

    public class ClubFilterRequest
    {
        public int? Id { get; set; }

        // DeparmentId ở đây dùng để lọc: nếu truyền, sẽ lấy Public Clubs + Private Clubs của DeparmentId này
        public int? DeparmentId { get; set; }

        // IsPublic ở đây chỉ dùng nếu DeparmentId không được truyền (chỉ muốn lọc Public/Private chung)
        public bool? IsPublic { get; set; }

        public string? Name { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }

        public string[]? SelectFields { get; set; }
    }
}
