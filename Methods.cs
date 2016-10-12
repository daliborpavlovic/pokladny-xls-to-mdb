using System;
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
        public static void CreateDatabase(string file)
        {
            CatalogClass cat = new CatalogClass();
            
            if (File.Exists(file))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception e)
                {

                    Log.WriteErrorLog("Error deleting old database file: " + e.Message);
                }
            }

            try
            {
                cat.Create(Methods.ConnectionString(file));
                Log.WriteLog("Database created: " + file);
            }
            catch (Exception e)
            {

                Log.WriteErrorLog("Error cretating new database file" + e.Message);
            }
            
        }

        public static string ConnectionString(string file)
        {
            return String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", file);
        }
    }
}
