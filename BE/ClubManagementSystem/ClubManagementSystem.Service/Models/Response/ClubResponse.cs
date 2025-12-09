using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagementSystem.Service.Models.Response
{
    public class ClubResponse
    {
        public int Id { get; set; }
        public int? DeparmentId { get; set; }
        public string? Name { get; set; }
        public bool? IsPublic { get; set; }
        public string? Description { get; set; }
        public DateOnly? EstablishedDate { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
