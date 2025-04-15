using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.CSVImports
{
    public class CSVImportBL : GenericRepositoryDL<CSVImport> , ICSVImportBL
    {
        public CSVImportBL(ApplicationDbContext context) : base(context)
        {
        }
    }
}
