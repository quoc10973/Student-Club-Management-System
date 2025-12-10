using System;
using System.ComponentModel.DataAnnotations;

namespace ClubManagementSystem.Service.Models.Request
{
    public class RegistrationRequest
    {
        [Required(ErrorMessage = "ClubId is required")]
        public int ClubId { get; set; }

        public DateTime? StartDay { get; set; }
        public DateTime? EndDay { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [MaxLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
        public string? Status { get; set; }

        [MaxLength(255, ErrorMessage = "Note cannot exceed 255 characters")]
        public string? Note { get; set; }
    }

    public class RegistrationFilterRequest
    {
        public int? Id { get; set; }
        public int? ClubId { get; set; }
        public string? Status { get; set; }
        public string? Note { get; set; }

        public string[]? SelectFields { get; set; }
    }
}


