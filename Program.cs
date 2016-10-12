using System;
using ADOX;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokladna
{
    class Program
    {
        static void Main(string[] args)
        {
            string sourceFile = @"Pokladna.mdb";
            string destinationFile = @"PokladnaToPohoda.mdb";

            // create new access dtb file
            Catalog catalog = Methods.CreateDatabase(destinationFile);

            // create tables in access dtb
            Methods.CreateTable(catalog, "Prijmy");
            Methods.CreateTable(catalog, "Vydaje");

            // prepare dataset for the source data
            DataSet dataset = Methods.GetData(sourceFile);



        }
    }
}
