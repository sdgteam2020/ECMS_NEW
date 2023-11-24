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
        public Task<int> UpdateChageComdByCorps(MapUnit Data);
        public Task<int> UpdateComdCorpsByDivs(MapUnit Data);
        public Task<int> UpdateComdCorpsDivsBybdes(MapUnit Data);
    }
}
