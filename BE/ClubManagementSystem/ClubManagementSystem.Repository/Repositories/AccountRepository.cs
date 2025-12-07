using ClubManagementSystem.Repository.Basic;
using ClubManagementSystem.Repository.DBContext;
using ClubManagementSystem.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagementSystem.Repository.Repositories
{
    public class AccountRepository : GenericRepository<Account>
    {
        public AccountRepository(ClubManagementSystemContext context) : base(context)
        {
        }

        public IQueryable<Account> GetFilteredAccount(Account account)
        {
            var query = _context.Accounts.AsQueryable();
            if(account.Id != 0)
                query = query.Where(a => a.Id == account.Id);
            if(account.Role != null)
                query = query.Where(a => a.Role.ToLower() == account.Role.ToLower());
            if(account.Status != null)
                query = query.Where(a => a.Status.Equals(account.Status));
            if (!string.IsNullOrEmpty(account.FullName))
                query = query.Where(a => a.FullName.Contains(account.FullName));
            if (!string.IsNullOrEmpty(account.Phone))
                query = query.Where(a => a.Phone.Contains(account.Phone));
            if (!string.IsNullOrEmpty(account.Username))
                query = query.Where(a => a.Username.Contains(account.Username));
            if (!string.IsNullOrEmpty(account.Email))
                query = query.Where(a => a.Email.Contains(account.Email));    
            return query.OrderBy(a => a.Id);
        }
    }
}
