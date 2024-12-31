using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IChangeHierarchyMasterDB
    {
        public Task<bool> UpdateChageComdByCorps(MapUnit Data);
        public Task<bool> UpdateComdCorpsByDivs(MapUnit Data);
        public Task<bool> UpdateComdCorpsDivsBybdes(MapUnit Data);
      
    }
}
