using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DataAccessLayer
{
    public class ComdDB : GenericRepositoryDL<MComd>, IComdDB
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DapperContext _contextDP;
        public ComdDB(ApplicationDbContext context, DapperContext contextDP) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
        }
      

      
        private readonly IConfiguration configuration;
        //public UserDB(IConfiguration configuration)
        //{
        //    this.configuration = configuration;
        //}

       

         public async Task<bool> GetByName(MComd DTo)
         {
            // && p.ComdId != DTo.ComdId && p.IsDeleted==true
            var ret = _context.MComd.Select(p => p.ComdName.ToUpper() == DTo.ComdName.ToUpper()).FirstOrDefault();
            return ret;
        }

        public async Task<int> GetByMaxOrder()
        {
            int ret = _context.MComd.Max(P => P.Orderby);
            return ret+1;
        }

        public async Task<int> GetComdIdbyOrderby(int OrderBy)
        {
            var ret= _context.MComd.Where(P => P.Orderby == OrderBy).Select(c=>c.ComdId).SingleOrDefault(); 
           
            return ret;
        }

        public async Task<IEnumerable<MComd>> GetAllByorder()
        {
            var ret = _context.MComd.OrderBy(x => x.Orderby).ToList();
            return ret;
        }
        public async Task<DTOTreeViewUnitResponse> GetBinaryTree(int Id)
        {
            try
            {

                string query = "Select ComdId,ComdName from MComd  where ComdId=@Id";
                string MCorps = " Select ComdId,CorpsId,CorpsName from MCorps  where ComdId=@Id";
                string MDiv = " Select CorpsId,DivId,DivName from MDiv  where ComdId=@Id";
                string MBde = " Select CorpsId,DivId,BdeId,BdeName from MBde  where ComdId=@Id";
                string MapUnit = " Select UnitMapId UnitId,ComdId,CorpsId,DivId,BdeId,UnitName from MapUnit inner join MUnit on MapUnit.UnitId=MUnit.UnitId  where ComdId=@Id";
                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<MComd>(query, new { Id });
                    var ret1 = await connection.QueryAsync<MCorps>(MCorps, new { Id });
                    var ret2 = await connection.QueryAsync<MDiv>(MDiv, new { Id });
                    var ret3 = await connection.QueryAsync<MBde>(MBde, new { Id });
                    var ret4 = await connection.QueryAsync<DTOMapUnitResponse>(MapUnit, new { Id });
                    DTOTreeViewUnitResponse dTOTreeViewUnitResponse = new DTOTreeViewUnitResponse();

                    dTOTreeViewUnitResponse.MComd  = (List<MComd>)ret;
                    dTOTreeViewUnitResponse.MCorps = (List<MCorps>)ret1;
                    dTOTreeViewUnitResponse.MDiv   = (List<MDiv>)ret2;
                    dTOTreeViewUnitResponse.MBde   = (List<MBde>)ret3;
                    dTOTreeViewUnitResponse.Unit   = (List<DTOMapUnitResponse>)ret4;

                    return dTOTreeViewUnitResponse;

                }
            }
            catch (Exception ex)
            {
                return null;
            }
            //var ret = (from uni in _context.MapUnit
            //               //where (unit.ComdId == 0 ? uni.ComdId == uni.ComdId : uni.ComdId == unit.ComdId)
            //               //&& (unit.CorpsId == 0 ? uni.CorpsId == uni.CorpsId : uni.CorpsId == unit.CorpsId)
            //               //&& (unit.DivId == 0 ? uni.DivId == uni.DivId : uni.DivId == unit.DivId)
            //               //&& (unit.BdeId == 0 ? uni.BdeId == uni.BdeId : uni.BdeId == unit.BdeId)
            //           join MUni in _context.MUnit on uni.UnitId equals MUni.UnitId
            //           join Com in _context.MComd on uni.ComdId equals Com.ComdId
            //           //   on new { uni.ComdId } equals new { Com.ComdId }
            //           join cor in _context.MCorps on uni.CorpsId equals cor.CorpsId
            //           join div in _context.MDiv on uni.DivId equals div.DivId
            //           join bde in _context.MBde on uni.BdeId equals bde.BdeId
            //           join pso in _context.MPso on uni.PsoId equals pso.PsoId
            //           join FmnBranch in _context.MFmnBranches on uni.FmnBranchID equals FmnBranch.FmnBranchID
            //           join SubDte in _context.MSubDte on uni.SubDteId equals SubDte.SubDteId
            //           where Com.ComdId==Id
            //           select new DTOMapUnitResponse
            //           {
            //               UnitMapId = uni.UnitMapId,
            //               UnitName = MUni.UnitName,
            //               IsVerify = MUni.IsVerify,
            //               UnitId = uni.UnitId,
            //               BdeId = bde.BdeId,
            //               BdeName = bde.BdeName,
            //               DivId = div.DivId,
            //               DivName = div.DivName,
            //               CorpsId = cor.CorpsId,
            //               CorpsName = cor.CorpsName,
            //               ComdName = Com.ComdName,
            //               ComdId = Com.ComdId,
            //               Suffix = MUni.Suffix,
            //               Sus_no = MUni.Sus_no,
            //               UnitType = uni.UnitType,
            //               PsoId = pso.PsoId,
            //               PSOName = pso.PSOName,
            //               FmnBranchID = FmnBranch.FmnBranchID,
            //               BranchName = FmnBranch.BranchName,
            //               SubDteId = SubDte.SubDteId,
            //               SubDteName = SubDte.SubDteName,
            //           }
            //         ).ToList(); ;


            
        }
    }
}