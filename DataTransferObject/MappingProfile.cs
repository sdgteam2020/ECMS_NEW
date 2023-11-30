using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            CreateMap<DTOBasicDetailCrtRequest, BasicDetail>();
            CreateMap<BasicDetail, DTOBasicDetailUpdRequest>();
            CreateMap<BasicDetail, DTOBasicDetailRequest>();
            CreateMap<BasicDetail, BasicDetailUpdVMPart1>();
            CreateMap<BasicDetail, BasicDetailUpdVMPart2>();

            CreateMap<BasicDetailCrtAndUpdVM, BasicDetail>();

            CreateMap<DTODocUploadCrtRequest, DocUpload>();
            CreateMap<DocUpload, DTODocUploadUpdRequest>();
        }
    }
}
