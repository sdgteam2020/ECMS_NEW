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
        public Task<int> UpdateChageComdByCorps(MapUnit Data);
        public Task<int> UpdateComdCorpsByDivs(MapUnit Data);
        public Task<int> UpdateComdCorpsDivsBybdes(MapUnit Data);
      
    }
}
