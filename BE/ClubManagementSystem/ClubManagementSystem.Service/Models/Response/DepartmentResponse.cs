using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagementSystem.Service.Models.Response
{
    public class DepartmentResponse
    {
        public int Id { get; set; }
        public string? CodeName { get; set; }
        public string? Name { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
