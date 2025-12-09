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
    public class ClubRepository : GenericRepository<Club>
    {
        public ClubRepository(ClubManagementSystemContext context) : base(context)
        {
        }

        public IQueryable<Club> GetFilteredClub(Club club, bool? isPublic, int? deparmentId)
        {
            var query = _context.Set<Club>().AsQueryable();

            if (club.Id != 0)
                query = query.Where(c => c.Id == club.Id);

            // Lọc theo DeparmentId nếu được cung cấp trong filter
            if (deparmentId.HasValue && deparmentId != 0)
            {
                // Logic: Lấy các Club Public HOẶC Club dành riêng cho DeparmentId này
                query = query.Where(c => c.IsPublic == true || c.DeparmentId == deparmentId);
            }
            // Nếu không có DeparmentId trong filter, chỉ lấy các Club Public
            else if (isPublic.HasValue)
            {
                query = query.Where(c => c.IsPublic == isPublic.Value);
            }

            if (!string.IsNullOrEmpty(club.Name))
                query = query.Where(c => c.Name.Contains(club.Name));

            if (!string.IsNullOrEmpty(club.Status))
                query = query.Where(c => c.Status.Equals(club.Status));

            return query.OrderBy(c => c.Id);
        }

        // Phương thức lọc đơn giản (chỉ dùng cho GetById/Update/Delete nếu không cần logic phức tạp)
        public IQueryable<Club> GetSimpleFilteredClub(Club club)
        {
            var query = _context.Set<Club>().AsQueryable();

            if (club.Id != 0)
                query = query.Where(c => c.Id == club.Id);

            if (club.DeparmentId.HasValue && club.DeparmentId != 0)
                query = query.Where(c => c.DeparmentId == club.DeparmentId);

            if (!string.IsNullOrEmpty(club.Name))
                query = query.Where(c => c.Name.Contains(club.Name));

            if (club.IsPublic.HasValue)
                query = query.Where(c => c.IsPublic == club.IsPublic);

            if (!string.IsNullOrEmpty(club.Status))
                query = query.Where(c => c.Status.Equals(club.Status));

            return query.OrderBy(c => c.Id);
        }
    }
}
