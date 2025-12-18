using ClubManagementSystem.Repository.Entities;
using ClubManagementSystem.Service.Models.Request;
using ClubManagementSystem.Service.Models.Response;
using Mapster;

namespace ClubManagementSystem.Service.Mappings
{
    public class ClubMembershipMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<ClubMembershipFilterRequest, ClubMembership>()
                  .IgnoreNullValues(true);

            config.NewConfig<ClubMembershipRequest, ClubMembership>();

            config.NewConfig<ClubMembership, ClubMembershipResponse>();
        }
    }
}












