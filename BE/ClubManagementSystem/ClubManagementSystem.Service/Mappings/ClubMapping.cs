using ClubManagementSystem.Repository.Entities;
using ClubManagementSystem.Service.Models.Request;
using ClubManagementSystem.Service.Models.Response;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagementSystem.Service.Mappings
{
    public class ClubMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // filter request -> entity
            config.NewConfig<ClubFilterRequest, Club>()
                  .IgnoreNullValues(true)
                  // Chỉ map các trường không liên quan đến logic đặc biệt trong GetAllClubs
                  .Map(dest => dest.Name, src => src.Name)
                  .Map(dest => dest.Status, src => src.Status)
                  .Map(dest => dest.CreatedAt, src => src.CreatedAt)
                  .Ignore(dest => dest.IsPublic) // Bỏ qua IsPublic để xử lý riêng trong Repository
                  .Ignore(dest => dest.DeparmentId); // Bỏ qua DeparmentId để xử lý riêng trong Repository

            // create/update request -> entity
            config.NewConfig<ClubRequest, Club>()
                .Map(dest => dest.EstablishedDate, src => src.EstablishedDate);

            // entity -> response
            config.NewConfig<Club, ClubResponse>();
        }
    }
}
