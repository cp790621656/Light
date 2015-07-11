using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace HttpServer
{
    class ServerMain
    {
        private static string m_Version = "1.0.0";
        private static string m_Ip = "127.0.0.1";
        private static int m_Port = 1423;


        static void Main(string[] args)
        {
            ShowLight();

            HttpServer httpServer = new LightHttpServer(m_Ip, m_Port);

            Thread httpThread = new Thread(new ThreadStart(httpServer.StartListen));
            httpThread.Start();

        }

        static void ShowLight()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;

            string logo = "-------------------------------------------------------------------------------\n" +
                    "|                                                                             \n" +
                    "| ||      || |||||||||| ||      || ||||||||||" + "  Light HttpServer version "+m_Version+"\n" +
                    "| ||      || ||     |_| ||      ||     ||    " + "  IP:" + m_Ip + "\n" +
                    "| ||      || ||         ||||||||||     ||    " + "  Port:" + m_Port + "\n" +
                    "| ||      || ||   ||||| ||      ||     ||            \n" +
                    "| ||      || ||      || ||      ||     ||             \n" +
                    "| ||||||| || |||||||||| ||      ||     ||    " + "  http://www.thisisgame.com.cn\n" +
                    "|                                                                             \n" +
                    "-------------------------------------------------------------------------------\n";
            Console.WriteLine(logo);
        }
    }
}
