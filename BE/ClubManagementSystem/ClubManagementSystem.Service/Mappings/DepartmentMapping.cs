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
    public class DepartmentMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // filter request -> entity
            config.NewConfig<DepartmentFilterRequest, Deparment>()
                  .IgnoreNullValues(true)
                  .Map(dest => dest.Name, src => src.Name); // Đảm bảo Name được map

            // create/update request -> entity
            config.NewConfig<DepartmentRequest, Deparment>();

            // entity -> response
            config.NewConfig<Deparment, DepartmentResponse>();
        }
    }
}
