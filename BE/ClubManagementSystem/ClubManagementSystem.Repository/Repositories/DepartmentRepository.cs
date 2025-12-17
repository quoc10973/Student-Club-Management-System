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
    public class DepartmentRepository : GenericRepository<Deparment>
    {
        public DepartmentRepository(ClubManagementSystemContext context) : base(context)
        {
        }

        public IQueryable<Deparment> GetFilteredDepartment(Deparment department)
        {
            var query = _context.Set<Deparment>().AsQueryable();

            if (department.Id != 0)
                query = query.Where(d => d.Id == department.Id);
            if (!string.IsNullOrEmpty(department.CodeName))
                query = query.Where(d => d.CodeName.Contains(department.CodeName));
            if (!string.IsNullOrEmpty(department.Name))
                query = query.Where(d => d.Name.Contains(department.Name));
            if (!string.IsNullOrEmpty(department.Status))
                query = query.Where(d => d.Status.Equals(department.Status));

            return query.OrderBy(d => d.Id);
        }

        public async Task<Deparment?> GetFirstActiveDepartmentAsync()
        {
            return await _context.Set<Deparment>()
                .Where(d => d.Status == "Active")
                .OrderBy(d => d.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<Deparment?> GetFirstDepartmentAsync()
        {
            return await _context.Set<Deparment>()
                .OrderBy(d => d.Id)
                .FirstOrDefaultAsync();
        }
    }
}
