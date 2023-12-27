using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DataTransferObject.Domain;
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

            CreateMap<DTODocUploadCrtRequest, DocUpload>();
            CreateMap<DocUpload, DTODocUploadUpdRequest>();
        }
    }
}
