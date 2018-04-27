using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slingshot.TheCity.Utilities;

namespace Slingshot.TheCity
{
    static class TestApplication
    {
        public static void Main( string[] args )
        {
            string sampleDate = "01/01/2001";
            DateTime startDate = Convert.ToDateTime( sampleDate );

            TheCityApi.ExportUsers( startDate );
        }

    }  
}
