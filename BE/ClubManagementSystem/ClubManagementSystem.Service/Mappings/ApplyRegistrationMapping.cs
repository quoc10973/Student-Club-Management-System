using ClubManagementSystem.Repository.Entities;
using ClubManagementSystem.Service.Models.Request;
using ClubManagementSystem.Service.Models.Response;
using Mapster;

namespace ClubManagementSystem.Service.Mappings
{
    public class ApplyRegistrationMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<ApplyRegistrationFilterRequest, ApplyRegistration>()
                  .IgnoreNullValues(true);

            config.NewConfig<ApplyRegistrationRequest, ApplyRegistration>();

            config.NewConfig<ApplyRegistration, ApplyRegistrationResponse>();
        }
    }
}














