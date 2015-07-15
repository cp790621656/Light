using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HttpServer
{
    public class LightHttpServer:HttpServer
    {
        public LightHttpServer(string ip,int port)
            :base(ip,port)
        {

        }

        /// <summary>
        /// 处理Get请求;
        /// </summary>
        /// <param name="p"></param>
        public override void HandleGetRequest(HttpProcessor processor)
        {
            Console.WriteLine("request: {0}", processor.m_HttpUrl);
            processor.WriteSuccess();

            processor.m_OutputStreamWriter.WriteLine("<html><body><h1>LightHttpServer</h1>");
            processor.m_OutputStreamWriter.WriteLine("Current Time: " + DateTime.Now.ToString());
            processor.m_OutputStreamWriter.WriteLine("url : {0}", processor.m_HttpUrl);

            //Post测试;
            processor.m_OutputStreamWriter.WriteLine("<form method=post action=/form>");
            processor.m_OutputStreamWriter.WriteLine("<input type=text name=foo value=foovalue>");
            processor.m_OutputStreamWriter.WriteLine("<input type=submit name=bar value=barvalue>");
            processor.m_OutputStreamWriter.WriteLine("</form>");
        }

        /// <summary>
        /// 处理Post请求;
        /// </summary>
        /// <param name="p"></param>
        /// <param name="inputStreamReader"></param>
        public override void HandlePostRequest(HttpProcessor processor, StreamReader inputStreamReader)
        {
            Console.WriteLine("POST request: {0}", processor.m_HttpUrl);

            string data = inputStreamReader.ReadToEnd();

            processor.m_OutputStreamWriter.WriteLine("<html><body><h1>test server</h1>");
            processor.m_OutputStreamWriter.WriteLine("<a href=/test>return</a><p>");
            processor.m_OutputStreamWriter.WriteLine("postbody: <pre>{0}</pre>", data);
        }
    }
}
