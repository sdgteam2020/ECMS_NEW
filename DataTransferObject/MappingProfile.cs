using AutoMapper;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.ViewModels;

namespace DataTransferObject
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BasicDetail, DTOBasicDetailRequest>();

            CreateMap<BasicDetailCrtAndUpdVM, BasicDetail>();
            CreateMap<BasicDetail, BasicDetailCrtAndUpdVM>();

        }
    }
}
