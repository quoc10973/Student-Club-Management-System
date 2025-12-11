using System;
using System.ComponentModel.DataAnnotations;

namespace ClubManagementSystem.Service.Models.Request
{
    public class ClubMembershipRequest
    {
        [Required(ErrorMessage = "StudentId is required")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "ClubId is required")]
        public int ClubId { get; set; }

        [Required(ErrorMessage = "ClubRole is required")]
        [MaxLength(20, ErrorMessage = "ClubRole cannot exceed 20 characters")]
        [RegularExpression("^(Leader|Member)$", ErrorMessage = "ClubRole must be Leader or Member")]
        public string? ClubRole { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [MaxLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
        [RegularExpression("^(Active|InActive)$", ErrorMessage = "Status must be Active or InActive")]
        public string? Status { get; set; }

        [MaxLength(255, ErrorMessage = "Note cannot exceed 255 characters")]
        public string? Note { get; set; }
    }

    public class ClubMembershipFilterRequest
    {
        public int? Id { get; set; }
        public int? StudentId { get; set; }
        public int? ClubId { get; set; }
        public string? ClubRole { get; set; }
        public string? Status { get; set; }
        public int? ApprovedBy { get; set; }
        public string? Note { get; set; }

        public string[]? SelectFields { get; set; }
    }
}


