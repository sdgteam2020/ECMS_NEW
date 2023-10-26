using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Helpers
{
    public class csConst
    {
        private static string csalt; // field
        public static string cSalt   // property
        {
            get { return csalt; }
            set { csalt = value; }
        }
    }
}
