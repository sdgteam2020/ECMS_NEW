﻿using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.BasicDet
{
    public class BasicDetailBL:GenericRepositoryDL<BasicDetail>,IBasicDetailBL
    {
        private readonly IBasicDetailDB _iBasicDetailDB;
        public BasicDetailBL(ApplicationDbContext context,IBasicDetailDB BasicDetail) : base(context)
        {
                _iBasicDetailDB = BasicDetail;
        }
        public Task<List<BasicDetailVM>> GetALLBasicDetail(int UserId, int step, int type)
        {
            return _iBasicDetailDB.GetALLBasicDetail(UserId ,step, type);
        }

        public Task<DTOBasicDetailsResponse> GetByBasicDetailsId(int BasicDetailId)
        {
            return _iBasicDetailDB.GetByBasicDetailsId(BasicDetailId);
        }
    }
}
