﻿using BusinessLogicsLayer.BasicDet;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.BasicDetTemp
{
    public class BasicDetailTempBL : GenericRepositoryDL<BasicDetailTemp>, IBasicDetailTempBL
    {
        private readonly IBasicDetailTempDB _iBasicDetailTempDB;
        public BasicDetailTempBL(ApplicationDbContext context, IBasicDetailTempDB BasicDetailTemp) : base(context)
        {
            _iBasicDetailTempDB = BasicDetailTemp;
        }
        public Task<List<DTOBasicDetailTempRequest>> GetALLBasicDetailTemp(int UserId)
        {
            return _iBasicDetailTempDB.GetALLBasicDetailTemp(UserId);
        }

        public Task<DTOBasicDetailTempRequest> GetALLBasicDetailTempByBasicDetailId(int UserId, int BasicDetailId)
        {
            return _iBasicDetailTempDB.GetALLBasicDetailTempByBasicDetailId(UserId, BasicDetailId);
        }
    }
}
