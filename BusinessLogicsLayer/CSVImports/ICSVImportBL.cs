using DataAccessLayer;
using DataTransferObject.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.CSVImports
{
    public interface IcsvImportBl : IGenericRepository<CSVImport>
    {
    }
}
