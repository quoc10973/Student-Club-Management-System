using System;

namespace ClubManagementSystem.Service.Models.Response
{
    public class ApplyRegistrationResponse
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int RegistrationId { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}






