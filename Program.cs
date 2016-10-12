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
            string tableIncomeName = "Prijmy";
            string tableOutcomeName = "Vydaje";

            // create new access dtb file
            Catalog catalog = Methods.CreateDatabase(destinationFile);

            // create tables in access dtb
            Methods.CreateTable(catalog, tableIncomeName);
            Methods.CreateTable(catalog, tableOutcomeName);

            // prepare dataset for the source data
            DataSet dataset = new DataSet();
            dataset = Methods.GetData(sourceFile, tableIncomeName, dataset);
            dataset = Methods.GetData(sourceFile, tableOutcomeName, dataset);

            // insert data from dataset into the destination file
            // Methods.InsertData(destinationFile);
        }
    }
}
