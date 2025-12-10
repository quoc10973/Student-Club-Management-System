using ClubManagementSystem.Repository.Basic;
using ClubManagementSystem.Repository.DBContext;
using ClubManagementSystem.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ClubManagementSystem.Repository.Repositories
{
    public class ApplyRegistrationRepository : GenericRepository<ApplyRegistration>
    {
        public ApplyRegistrationRepository(ClubManagementSystemContext context) : base(context)
        {
        }

        public IQueryable<ApplyRegistration> GetFilteredApplyRegistration(ApplyRegistration apply)
        {
            var query = _context.ApplyRegistrations.AsQueryable();

            if (apply.Id != 0)
                query = query.Where(a => a.Id == apply.Id);
            if (apply.StudentId != 0)
                query = query.Where(a => a.StudentId == apply.StudentId);
            if (apply.RegistrationId != 0)
                query = query.Where(a => a.RegistrationId == apply.RegistrationId);
            if (!string.IsNullOrEmpty(apply.Status))
                query = query.Where(a => a.Status.Equals(apply.Status));

            return query.OrderBy(a => a.Id);
        }
    }
}


