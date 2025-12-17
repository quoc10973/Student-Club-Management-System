using System;

namespace ClubManagementSystem.Service.Models.Response
{
    public class RegistrationResponse
    {
        public int Id { get; set; }
        public int ClubId { get; set; }
        public DateTime? StartDay { get; set; }
        public DateTime? EndDay { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}






