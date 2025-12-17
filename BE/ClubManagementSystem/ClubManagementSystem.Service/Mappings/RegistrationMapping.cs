using ClubManagementSystem.Repository.Entities;
using ClubManagementSystem.Service.Models.Request;
using ClubManagementSystem.Service.Models.Response;
using Mapster;

namespace ClubManagementSystem.Service.Mappings
{
    public class RegistrationMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<RegistrationFilterRequest, Registration>()
                  .IgnoreNullValues(true);

            config.NewConfig<RegistrationRequest, Registration>();

            config.NewConfig<Registration, RegistrationResponse>();
        }
    }
}






