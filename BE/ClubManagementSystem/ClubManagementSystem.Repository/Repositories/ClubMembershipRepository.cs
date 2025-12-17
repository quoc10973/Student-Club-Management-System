using ClubManagementSystem.Repository.Basic;
using ClubManagementSystem.Repository.DBContext;
using ClubManagementSystem.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace ClubManagementSystem.Repository.Repositories
{
    public class ClubMembershipRepository : GenericRepository<ClubMembership>
    {
        public ClubMembershipRepository(ClubManagementSystemContext context) : base(context)
        {
        }

        public IQueryable<ClubMembership> GetFilteredClubMembership(ClubMembership membership)
        {
            var query = _context.ClubMemberships.AsQueryable();

            if (membership.Id != 0)
                query = query.Where(m => m.Id == membership.Id);
            if (membership.StudentId != 0)
                query = query.Where(m => m.StudentId == membership.StudentId);
            if (membership.ClubId != 0)
                query = query.Where(m => m.ClubId == membership.ClubId);
            if (!string.IsNullOrEmpty(membership.ClubRole))
                query = query.Where(m => m.ClubRole.Equals(membership.ClubRole));
            if (!string.IsNullOrEmpty(membership.Status))
                query = query.Where(m => m.Status.Equals(membership.Status));
            if (!string.IsNullOrEmpty(membership.Note))
                query = query.Where(m => m.Note.Contains(membership.Note));
            if (membership.ApprovedBy.HasValue)
                query = query.Where(m => m.ApprovedBy == membership.ApprovedBy);

            return query.OrderBy(m => m.Id);
        }

        public Task<bool> AnyAsync(Expression<Func<ClubMembership, bool>> predicate)
        {
            return _context.ClubMemberships.AnyAsync(predicate);
        }

        public Task<ClubMembership> FirstOrDefaultAsync(Expression<Func<ClubMembership, bool>> predicate)
        {
            return _context.ClubMemberships.FirstOrDefaultAsync(predicate);
        }
    }
}


