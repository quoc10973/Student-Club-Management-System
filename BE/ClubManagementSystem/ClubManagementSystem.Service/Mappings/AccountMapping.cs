using ClubManagementSystem.Repository.Entities;
using ClubManagementSystem.Service.Models.Request;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagementSystem.Service.Mappings
{
    public class AccountMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // filter request -> entity
            config.NewConfig<AccountFilterRequest, Account>()
                  .IgnoreNullValues(true);

            // create/update request -> entity
            config.NewConfig<AccountRequest, Account>();
        }

    }
}
