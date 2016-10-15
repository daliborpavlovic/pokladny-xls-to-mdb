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
            string destinationFile = @"C:\Users\dalibor.pavlovic\Documents\temp\Pokladna\Pokladna\bin\Debug\PokladnaToPohoda.mdb";
            string[] tableNames = { "Prijmy", "Vydaje" };

            // create destination database file
            Catalog catalog = Methods.CreateDatabase(destinationFile);

            // for each table name
            foreach (string tableName in tableNames)
            {
                // create table in destination database
                Methods.CreateTable(catalog, tableName);

                // get data from the source database
                DataSet dataset = Methods.GetData(sourceFile, tableName);

                // insert data from dataset into the destination file
                Methods.InsertData(destinationFile, tableName, dataset);
            }            
        }
    }
}
