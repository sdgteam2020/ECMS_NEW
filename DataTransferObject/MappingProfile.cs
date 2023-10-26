using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;

namespace DataTransferObject
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DTOProfileDataCrtRequest, ProfileData>();
            CreateMap<ProfileData, DTOProfileDataUpdRequest>();
            CreateMap<DTOProfileDataUpdRequest, ProfileData>();
            CreateMap<ProfileData, DTOProfileDataRequest>();

            CreateMap<DTOBasicDetailCrtRequest, BasicDetail>();
            CreateMap<BasicDetail, DTOBasicDetailUpdRequest>();
            CreateMap<BasicDetail, DTOBasicDetailRequest>();

            CreateMap<DTODocUploadCrtRequest, DocUpload>();
            CreateMap<DocUpload, DTODocUploadUpdRequest>();
        }
    }
}
