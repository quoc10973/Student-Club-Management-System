using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagementSystem.Service.Models.Response
{
    public class StudentResponse
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int DeparmentId { get; set; }
        public string Status { get; set; }
        public string Code { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}





