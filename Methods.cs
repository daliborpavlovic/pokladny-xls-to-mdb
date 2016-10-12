using System;
using System.Data;
using System.Data.OleDb;
using ADOX;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokladna
{
    public static class Methods
    {
        public static string ConnectionString(string file)
        {
            return String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", file);
        }

        public static Catalog CreateDatabase(string file)
        {
            Catalog catalog = new Catalog();
            
            if (File.Exists(file))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception e)
                {

                    Log.WriteErrorLog("Cannot delete old database file: " + e.Message);
                }
            }

            try
            {
                catalog.Create(Methods.ConnectionString(file));
                Log.WriteLog("Database created: " + file);
            }
            catch (Exception e)
            {

                Log.WriteErrorLog("Cannot create new database file: " + e.Message);
            }

            return catalog;
        }

        public static void CreateTable(Catalog catalog, string tableName)
        {
            Table table = new Table();
            table.Name = tableName;

            table.Columns.Append("Id", DataTypeEnum.adInteger);
            table.Columns.Append("Datum", DataTypeEnum.adDate);
            table.Columns.Append("CisloDokladu", DataTypeEnum.adVarWChar, 7);
            table.Columns.Append("Popis", DataTypeEnum.adVarWChar, 100);
            table.Columns.Append("Pohyb", DataTypeEnum.adInteger);
            table.Columns.Append("Castka", DataTypeEnum.adInteger);

            try
            {
                catalog.Tables.Append(table);
            }
            catch (Exception e)
            {

                Log.WriteErrorLog("Cannot create table in the new database file: " + e.Message);
            }

        }

        public static DataSet GetData(string file, string tableName, DataSet dataset)
        {
            string query = String.Format("SELECT * FROM Data WHERE CisloDokladu LIKE '{0}%'", tableName.Substring(0,1));
            OleDbConnection connection = new OleDbConnection(Methods.ConnectionString(file));

            try
            {
                using (var adapter = new OleDbDataAdapter(query, connection))
                {
                    adapter.TableMappings.Add("Table", tableName);
                    adapter.Fill(dataset);
                }

            }
            catch (Exception e)
            {
                Log.WriteErrorLog("Cannot retrieve data from Access database: " + e.Message);
            }

            return dataset;
        }
    }
}
