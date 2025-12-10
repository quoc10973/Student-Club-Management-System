using ClubManagementSystem.Repository.Basic;
using ClubManagementSystem.Repository.DBContext;
using ClubManagementSystem.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ClubManagementSystem.Repository.Repositories
{
    public class RegistrationRepository : GenericRepository<Registration>
    {
        public RegistrationRepository(ClubManagementSystemContext context) : base(context)
        {
        }

        public IQueryable<Registration> GetFilteredRegistration(Registration registration)
        {
            var query = _context.Registrations.AsQueryable();

            if (registration.Id != 0)
                query = query.Where(r => r.Id == registration.Id);
            if (registration.ClubId != 0)
                query = query.Where(r => r.ClubId == registration.ClubId);
            if (!string.IsNullOrEmpty(registration.Status))
                query = query.Where(r => r.Status.Equals(registration.Status));
            if (!string.IsNullOrEmpty(registration.Note))
                query = query.Where(r => r.Note.Contains(registration.Note));

            return query.OrderBy(r => r.Id);
        }

        public async Task<bool> HasApplyRegistrationsAsync(int registrationId)
        {
            return await _context.ApplyRegistrations.AnyAsync(a => a.RegistrationId == registrationId);
        }

        public async Task<bool> HasPendingApplyRegistrationsAsync(int registrationId)
        {
            return await _context.ApplyRegistrations.AnyAsync(a => a.RegistrationId == registrationId && a.Status == "Pending");
        }

        public async Task<int> DeleteApplyRegistrationsAsync(int registrationId)
        {
            var applies = await _context.ApplyRegistrations.Where(a => a.RegistrationId == registrationId).ToListAsync();
            if (applies.Count == 0) return 0;
            _context.ApplyRegistrations.RemoveRange(applies);
            return await _context.SaveChangesAsync();
        }
    }
}

