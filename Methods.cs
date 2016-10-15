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

        public static Column CreateColumn(Table table, string columnName)
        {
            Column column = new Column();
            column.Name = columnName;
            column.Attributes = ColumnAttributesEnum.adColNullable;
            return column;
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
                catalog.Create(ConnectionString(file));
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

            Column id = CreateColumn(table, "Id");
            Column datum = CreateColumn(table, "Datum");
            Column cisloDokladu = CreateColumn(table, "CisloDokladu");
            Column popis = CreateColumn(table, "Popis");
            Column pohyb = CreateColumn(table, "Pohyb");
            Column castka = CreateColumn(table, "Castka");

            cisloDokladu.DefinedSize = 7;
            popis.DefinedSize = 100;

            id.Type = DataTypeEnum.adInteger;
            datum.Type = DataTypeEnum.adDate;
            cisloDokladu.Type = DataTypeEnum.adVarWChar;
            popis.Type = DataTypeEnum.adVarWChar;
            pohyb.Type = DataTypeEnum.adInteger;
            castka.Type = DataTypeEnum.adInteger;

            try
            {
                table.Columns.Append(id);
                table.Columns.Append(datum);
                table.Columns.Append(cisloDokladu);
                table.Columns.Append(popis);
                table.Columns.Append(pohyb);
                table.Columns.Append(castka);
            }
            catch (Exception e)
            {

                Log.WriteErrorLog(String.Format("Cannot create column in the database table {0}: ", table) + e.Message);
            }

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
            DataSet dataSet = new DataSet();

            try
            {
                using (var adapter = new OleDbDataAdapter(query, connection))
                {
                    adapter.TableMappings.Add("Table", tableName);
                    adapter.Fill(dataSet);
                }

            }
            catch (Exception e)
            {
                Log.WriteErrorLog(String.Format("Cannot retrieve data from Access database file {0}: ", file) + e.Message);
            }

            return SetDataRowsAdded(dataSet, tableName);
        }

        public static void InsertData(string file, string tableName, DataSet dataSet)
        {
            //string command = "INSERT INTO @Table (Id, Datum, CisloDokladu, Popis, Pohyb, Castka) VALUES (@Id, @Datum, @CisloDokladu, @Popis, @Pohyb, @Castka)";
            string insertString = String.Format("INSERT INTO {0}([Popis]) VALUES (?);", tableName); //@Table, @CisloDokladu, @Id
            var adapter = new OleDbDataAdapter();

            try
            {
                using (var connection = new OleDbConnection(Methods.ConnectionString(file)))
                {

                    // var command = new OleDbCommand(insertString, connection);
                    // adapter.InsertCommand = command;
                    adapter.InsertCommand = new OleDbCommand(insertString);
                    adapter.InsertCommand.Connection = connection;
                    adapter.InsertCommand.Parameters.Add("param1", OleDbType.VarWChar, 100, "Popis");

                    //adapter.InsertCommand = new OleDbCommand(command, connection);
                    //adapter.InsertCommand.Parameters.Add("@Table", OleDbType.VarChar, 6, "Table");
                    //adapter.InsertCommand.Parameters.Add("CisloDokladu", OleDbType.VarChar, 100, "CisloDokladu");
                    //adapter.InsertCommand.Parameters.Add("@Id", OleDbType.VarChar, 1000, "Id");
                    //adapter.InsertCommand.Parameters.Add("@Datum", OleDbType.DBDate);
                    //adapter.InsertCommand.Parameters.Add("@CisloDokladu", OleDbType.VarChar, 7, "CisloDokladu");
                    //adapter.InsertCommand.Parameters.Add("@Popis", OleDbType.VarChar, 100, "Popis");
                    //adapter.InsertCommand.Parameters.Add("@Pohyb", OleDbType.VarChar, 6, "Pohyb");
                    //adapter.InsertCommand.Parameters.Add("@Castka", OleDbType.VarChar, 6, "Castka");

                    connection.Open();
                    try
                    {
                        adapter.Update(dataSet, tableName);
                    }
                    catch (Exception e)
                    {

                        Log.WriteErrorLog(String.Format("Cannot insert data into destination Access database file {0}: ", file) + e.Message);
                    }
                }
           
            }
            catch (Exception e)
            {
                Log.WriteErrorLog("Database connection error: " + e.Message);
            }
        }

        public static DataSet SetDataRowsAdded(DataSet dataSet, string tableName)
        {
            foreach (DataRow row in dataSet.Tables[tableName].Rows)
            {
                row.SetAdded();
            }

            return dataSet;
            
        }
    }
}
