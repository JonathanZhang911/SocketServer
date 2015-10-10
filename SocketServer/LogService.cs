using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SocketServer
{
    public class LogService
    {
        private static string logFileLocation = "E:\\ServerLog\\";
        public static void Log(Message.Message data)
        {
            try
            {
                StreamWriter sw = File.AppendText(logFileLocation + "log.txt");
                string temp = "Type" + data.messageType.ToString() + " Time:" + data.time + " User:" + data.userName + " Room:" + data.room + " Text:" + data.text;
                sw.WriteLine(temp);
                sw.Flush();
                sw.Close();
            }
            catch
            {

            }
        }
        public static void LogError(string error)
        {
            try
            {
                StreamWriter sw = File.AppendText(logFileLocation + "error.txt");
                sw.WriteLine(System.DateTime.Now.ToString() + "  " + error);
                sw.Flush();
                sw.Close();
            }
            catch
            {

            }


        }
    }
}
