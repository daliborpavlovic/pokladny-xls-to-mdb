using System;
using ADOX;
using System.IO;
using System.Data.OleDb;
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
            // create new access dtb file
            string file = @"PokladnaToPohoda.mdb";
            Methods.CreateDatabase(file);


            // create table in access dtb
            //using (var connection = new OleDbConnection(Methods.ConnectionString(file)))
            //{

            //}

            Catalog cat = new Catalog();
            Table tableIncome = new Table();
            Table tableOutcome = new Table();

            tableIncome.Name = "Prijmy";
            tableOutcome.Name = "Vydaje";
        }
    }
}
