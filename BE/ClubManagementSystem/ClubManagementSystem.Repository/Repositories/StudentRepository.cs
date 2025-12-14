using ClubManagementSystem.Repository.Basic;
using ClubManagementSystem.Repository.DBContext;
using ClubManagementSystem.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagementSystem.Repository.Repositories
{
    public class StudentRepository : GenericRepository<Student>
    {
        public StudentRepository(ClubManagementSystemContext context) : base(context)
        {
        }

        public IQueryable<Student> GetFilteredStudent(Student student)
        {
            var query = _context.Students.AsQueryable();
            if(student.Id != 0)
                query = query.Where(s => s.Id == student.Id);
            if(student.AccountId != 0)
                query = query.Where(s => s.AccountId == student.AccountId);
            if(student.DeparmentId != 0)
                query = query.Where(s => s.DeparmentId == student.DeparmentId);
            if (!string.IsNullOrEmpty(student.Status))
                query = query.Where(s => s.Status.Equals(student.Status));
            if (!string.IsNullOrEmpty(student.Code))
                query = query.Where(s => s.Code.Contains(student.Code));
            return query.OrderBy(s => s.Id);
        }
    }
}





