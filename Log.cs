using System;

using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokladna
{
    public static class Log
    {
        public static void WriteLog(string message)
        {
            string file = "log.txt";
            string newLine = "\r\n";
            using (StreamWriter sw = File.AppendText(file))
            {
                message = DateTime.Now.ToString("g") + " " + message.Trim() + newLine;
                sw.Write(message);
                Console.WriteLine(message);
            }
        }

        public static void WriteErrorLog(string message)
        {
            Log.WriteLog("ERROR: " + message);
            Environment.Exit(1);
        }

        public static void WriteWarningLog(string message)
        {
            Log.WriteLog("WARNING: " + message);
        }
    }
}
