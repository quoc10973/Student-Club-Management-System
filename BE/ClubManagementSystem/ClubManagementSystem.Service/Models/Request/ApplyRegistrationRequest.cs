using System;
using System.ComponentModel.DataAnnotations;

namespace ClubManagementSystem.Service.Models.Request
{
    public class ApplyRegistrationRequest
    {
        [Required(ErrorMessage = "StudentId is required")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "RegistrationId is required")]
        public int RegistrationId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [MaxLength(10, ErrorMessage = "Status cannot exceed 10 characters")]
        public string? Status { get; set; }
    }

    public class ApplyRegistrationFilterRequest
    {
        public int? Id { get; set; }
        public int? StudentId { get; set; }
        public int? RegistrationId { get; set; }
        public string? Status { get; set; }

        public string[]? SelectFields { get; set; }
    }
}


