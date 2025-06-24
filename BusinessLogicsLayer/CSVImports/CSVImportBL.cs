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
    public class CsvImportBl : GenericRepositoryDL<CSVImport> , IcsvImportBl
    {
        public CsvImportBl(ApplicationDbContext context) : base(context)
        {
        }
    }
}
