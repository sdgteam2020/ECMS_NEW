using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class ChangeHierarchyMasterDB: IChangeHierarchyMasterDB
    {
        private readonly DapperContext _contextDP;
        public ChangeHierarchyMasterDB(DapperContext dapperContext)
        {
            _contextDP = dapperContext;
        }
        public async Task<int> UpdateChageComdByCorps(MapUnit Data)
        {
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    connection.Execute("update MBde set ComdId=@ComdId where CorpsId=@CorpsId", new { comdId = Data.ComdId, corpsId = Data.CorpsId });
                    connection.Execute("update MDiv set ComdId=@ComdId where CorpsId=@CorpsId", new { comdId = Data.ComdId, corpsId = Data.CorpsId });
                    connection.Execute("update MapUnit set ComdId=@ComdId where CorpsId=@CorpsId", new { comdId = Data.ComdId, corpsId = Data.CorpsId });
                }


            }
            catch (Exception ex) { }

            return 1;
        }
        public async Task<int> UpdateComdCorpsByDivs(MapUnit Data)
        {
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    connection.Execute("update MBde set ComdId=@ComdId,CorpsId=@CorpsIdwhere DivId=@DivId", new { comdId = Data.ComdId, corpsId = Data.CorpsId, divId=Data.DivId }); 
                    connection.Execute("update MapUnit set ComdId=@ComdId,CorpsId=@CorpsId where DivId=@DivId", new { comdId = Data.ComdId, corpsId = Data.CorpsId, divId = Data.DivId });
                }


            }
            catch (Exception ex) { }

            return 1;
        }
        public async Task<int> UpdateComdCorpsDivsBybdes(MapUnit Data)
        {
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                   
                    connection.Execute("update MapUnit set ComdId=@ComdId,CorpsId=@CorpsId,DivId=@DivId where BdeId=@BdeId", new { comdId = Data.ComdId, corpsId = Data.CorpsId, divId = Data.DivId, bdeId=Data.BdeId });
                }


            }
            catch (Exception ex) { }

            return 1;
        }
    }
}
