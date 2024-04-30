using BusinessLogicsLayer.Bde;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer;
using DataTransferObject.Domain.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.Response;
using DataTransferObject.Requests;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Dapper.SqlMapper;
using DapperRepo.Core.Constants;

namespace BusinessLogicsLayer.Unit
{
    public class UnitBL : GenericRepositoryDL<MUnit>, IUnitBL
    {
        private readonly IUnitDB _UnitDB;
        

        public UnitBL(ApplicationDbContext context, IUnitDB UnitDB) : base(context)
        {
            _UnitDB = UnitDB;
            
        }
       
        public async Task<List<MUnit>> GetAllUnit(string Sus_no)
        {
            //List<MUnit> List=await _UnitDB.GetAllUnit(Sus_no);
            //List<MUnit> Ret=new List<MUnit>();
            //foreach (MUnit item in List)
            //{
            //  //  item.UnitName=Encrypt.DecryptParameter(item.UnitName);
            //   // item.Sus_no = Encrypt.DecryptParameter(item.Sus_no);
            //    Ret.Add(item);
            //}
            return await _UnitDB.GetAllUnit(Sus_no);
        }

        public Task<bool> GetByName(MUnit Data)
        {
            return _UnitDB.GetByName(Data); 
        }

        public async Task<MUnit?> GetBySusNo(string Sus_no)
        {
            return await _UnitDB.GetBySusNo(Sus_no);
        }
        public async Task<bool> FindSusNo(string Sus_no)
        {
            return await _UnitDB.FindSusNo(Sus_no);
        }
        public async Task<bool?> GetBySusNoWithUnitId(string Sus_no, int UnitId)
        {
            return await _UnitDB.GetBySusNoWithUnitId(Sus_no, UnitId);
        }
        public async Task<List<DTOUnitResponse>?> GetTopBySUSNo(string SUSNo)
        {
            return await _UnitDB.GetTopBySUSNo(SUSNo);
        }
        public async Task<DTOUnitIdCheckInFKTableResponse?> UnitIdCheckInFKTable(int UnitId)
        {
            return await _UnitDB.UnitIdCheckInFKTable(UnitId);
        }
    }
}
