using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace HttpServer
{
    public abstract class HttpServer
    {
        private string m_IpAddress = string.Empty;//IP地址;
        protected int m_Port; //端口号;

        TcpListener m_TcpListener = null;

        bool m_isActive = true;

        public HttpServer(string ipaddress,int port)
        {
            m_IpAddress = ipaddress;
            m_Port = port;
        }

        public void StartListen()
        {

            m_TcpListener = new TcpListener(IPAddress.Parse(m_IpAddress), m_Port);

            //开始监听，这里有重载可以设置最大挂起数;
            m_TcpListener.Start();

            while (m_isActive)
            {
                //等待客户端连接;
                TcpClient tcpClient = m_TcpListener.AcceptTcpClient();

                //客户端连接之后，开始对连接数据进行处理;
                HttpProcessor processor = new HttpProcessor(tcpClient, this);
                processor.Process();
            }

        }

        /// <summary>
        /// 处理Get操作;
        /// </summary>
        /// <param name="p"></param>
        public abstract void HandleGetRequest(HttpProcessor p);

        /// <summary>
        /// 处理Post操作;
        /// </summary>
        /// <param name="p"></param>
        /// <param name="inputStreamReader"></param>
        public abstract void HandlePostRequest(HttpProcessor p, StreamReader inputStreamReader);
    }
}
