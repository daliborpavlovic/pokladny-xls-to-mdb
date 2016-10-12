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
            return String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}", file);
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

        public static DataSet GetData(string file, string tableName)
        {
            string query = String.Format("SELECT * FROM Data WHERE CisloDokladu LIKE '{0}%'", tableName.Substring(0,1));
            OleDbConnection connection = new OleDbConnection(Methods.ConnectionString(file));
            DataSet dataset = new DataSet();

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
                Log.WriteErrorLog(String.Format("Cannot retrieve data from Access database file {0}: ", file) + e.Message);
            }

            return dataset;
        }

        public static void InsertData(string file, string tableName, DataSet dataset)
        {
            //string command = "INSERT INTO @Table (Id, Datum, CisloDokladu, Popis, Pohyb, Castka) VALUES (@Id, @Datum, @CisloDokladu, @Popis, @Pohyb, @Castka)";
            string insertString = String.Format("INSERT INTO {0} (Id) VALUES (?)", tableName); //@Table, @CisloDokladu
            
            try
            {
                using (var connection = new OleDbConnection(Methods.ConnectionString(file)))
                {
                    connection.Open();
                    var adapter = new OleDbDataAdapter();
                    var command = new OleDbCommand(insertString, connection);
                    var table = dataset.Tables[tableName];

                    //command.Parameters.Add("CisloDokladu", OleDbType.LongVarChar, 100, dataset.Tables[tableName].Columns["CisloDokladu"].ToString() ?? DBNull.Value.ToString());
                    command.Parameters.Add("Id", OleDbType.Integer);

                    //adapter.InsertCommand = new OleDbCommand(command, connection);
                    //adapter.InsertCommand.Parameters.Add("@Table", OleDbType.VarChar, 6, "Table");
                    //adapter.InsertCommand.Parameters.Add("CisloDokladu", OleDbType.VarChar, 100, "CisloDokladu");
                    //adapter.InsertCommand.Parameters.Add("@Id", OleDbType.VarChar, 1000, "Id");
                    //adapter.InsertCommand.Parameters.Add("@Datum", OleDbType.DBDate);
                    //adapter.InsertCommand.Parameters.Add("@CisloDokladu", OleDbType.VarChar, 7, "CisloDokladu");
                    //adapter.InsertCommand.Parameters.Add("@Popis", OleDbType.VarChar, 100, "Popis");
                    //adapter.InsertCommand.Parameters.Add("@Pohyb", OleDbType.VarChar, 6, "Pohyb");
                    //adapter.InsertCommand.Parameters.Add("@Castka", OleDbType.VarChar, 6, "Castka");

                    adapter.InsertCommand = command;

                    try
                    {
                        adapter.InsertCommand.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {

                        Log.WriteErrorLog(String.Format("Test {0}: ", file) + e.Message);
                    }
                    
                    //adapter.Update(dataset.Tables[tableName]);

                }
           
            }
            catch (Exception e)
            {
                Log.WriteErrorLog(String.Format("Cannot insert data into destination Access database file {0}: ", file) + e.Message);
            }
        }
    }
}
