using DataTransferObject.Domain.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Master
{
    public interface IChangeHierarchyMasterBL
    {
        public Task<bool> UpdateChageComdByCorps(MapUnit Data);
        public Task<bool> UpdateComdCorpsByDivs(MapUnit Data);
        public Task<bool> UpdateComdCorpsDivsBybdes(MapUnit Data);
    }
}
