using System;

namespace ClubManagementSystem.Service.Models.Response
{
    public class ClubMembershipResponse
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int ClubId { get; set; }
        public string ClubRole { get; set; }
        public string Status { get; set; }
        public DateTime? RequestedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public int? ApprovedBy { get; set; }
        public string Note { get; set; }
    }
}




