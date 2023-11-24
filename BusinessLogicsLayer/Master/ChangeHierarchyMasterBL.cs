using DataAccessLayer.BaseInterfaces;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.Domain.Master;

namespace BusinessLogicsLayer.Master
{
    public class ChangeHierarchyMasterBL: IChangeHierarchyMasterBL
    {
        private readonly IChangeHierarchyMasterDB _changeHierarchyMasterDB;
        public ChangeHierarchyMasterBL(IChangeHierarchyMasterDB changeHierarchyMasterDB)
        {
            _changeHierarchyMasterDB = changeHierarchyMasterDB;
        }
      

        public Task<int> UpdateChageComdByCorps(MapUnit Data)
        {
          return  _changeHierarchyMasterDB.UpdateChageComdByCorps(Data);
        }

        public Task<int> UpdateComdCorpsByDivs(MapUnit Data)
        {
            return _changeHierarchyMasterDB.UpdateComdCorpsByDivs(Data);
        }

        public Task<int> UpdateComdCorpsDivsBybdes(MapUnit Data)
        {
            return _changeHierarchyMasterDB.UpdateComdCorpsDivsBybdes(Data);
        }
    }
}
