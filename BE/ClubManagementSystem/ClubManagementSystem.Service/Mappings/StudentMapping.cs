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
    public class StudentMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // filter request -> entity
            config.NewConfig<StudentFilterRequest, Student>()
                  .IgnoreNullValues(true);

            // create/update request -> entity
            config.NewConfig<StudentRequest, Student>();

            // entity -> response
            config.NewConfig<Student, StudentResponse>();
        }
    }
}















