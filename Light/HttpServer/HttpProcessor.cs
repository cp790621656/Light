using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections;

namespace HttpServer
{
    public class HttpProcessor
    {
        public TcpClient m_TcpClient;

        public HttpServer m_HttpServer;

        public Stream m_InputStream;
        public StreamWriter m_OutputStreamWriter;

        public string m_HttpMethod;
        public string m_HttpUrl;
        public string m_HttpProtocalVersion;
        public Hashtable m_HttpHeadersTable = new Hashtable();

        private static int MAX_POST_SIZE = 10 * 1024 * 1024;

        public HttpProcessor(TcpClient tcpClient,HttpServer httpServer)
        {
            m_TcpClient = tcpClient;
            m_HttpServer = httpServer;
        }

        /// <summary>
        /// 处理数据;
        /// </summary>
        public void Process()
        {
            m_InputStream = new BufferedStream(m_TcpClient.GetStream());

            m_OutputStreamWriter = new StreamWriter(new BufferedStream(m_TcpClient.GetStream())); 

            try
            {
                //解析客户端 消息头;
                ParseRequest();

                //解析 请求头;
                ParseHeaders();

                //分别处理Get 和 Post 请求;
                if(m_HttpMethod.Equals("GET"))
                {
                    m_HttpServer.HandleGetRequest(this);
                }
                else if(m_HttpMethod.Equals("POST"))
                {
                    Console.WriteLine("Process Post data Start");
                    int contentLen = 0;
                    MemoryStream ms = new MemoryStream();

                    //是否包含 长度信息;
                    if(m_HttpHeadersTable.ContainsKey("Content-Length"))
                    {
                        contentLen = Convert.ToInt32(m_HttpHeadersTable["Content-Length"]);

                        //如果Post过来的数据太多，就报异常……;
                        if(contentLen>MAX_POST_SIZE)
                        {
                            throw new Exception(string.Format("Post Content-Length{0} too big for LightHttpServer", contentLen));
                        }

                        //如果Post数据OK，就接收;
                        byte[] buf = new byte[4096];
                        int toread = contentLen;
                        while(toread>0)
                        {
                            Console.WriteLine("start read post data,toread={0}", toread);

                            int numread = m_InputStream.Read(buf, 0, Math.Min(4096, toread));

                            Console.WriteLine("read finish,numread={0}", numread);

                            if(numread==0)
                            {
                                if(toread==0)
                                {
                                    break;
                                }
                                else
                                {
                                    throw new Exception("client disconnected during post");
                                }
                            }

                            toread -= numread;
                            ms.Write(buf, 0, numread);
                        }

                        ms.Seek(0, SeekOrigin.Begin);
                    }

                    Console.WriteLine("get post data end");

                    //传给HttpServer实例来处理逻辑;
                    m_HttpServer.HandlePostRequest(this, new StreamReader(ms));   
                }


            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception:" + ex.ToString());
                WriteFailure();
            }

            m_OutputStreamWriter.Flush();
            m_InputStream = null;
            m_OutputStreamWriter = null;
            m_TcpClient.Close();
        }

        /// <summary>
        /// 解析请求;
        /// </summary>
        public void ParseRequest()
        {
            string request = StreamReadLine(m_InputStream);

            string[] httpHeads = request.Split(' ');
            if (httpHeads.Length != 3)
            {
                throw new Exception("invalid http request line");
            }

            m_HttpMethod = httpHeads[0].ToUpper();
            m_HttpUrl = httpHeads[1];
            m_HttpProtocalVersion = httpHeads[2];
        }

        /// <summary>
        /// 解析数据头;
        /// </summary>
        public void ParseHeaders()
        {
            string line = string.Empty;
            while ((line == StreamReadLine(m_InputStream)) != null)
            {
                if (line.Equals(""))
                {
                    Console.WriteLine("got headers");
                    return;
                }

                int separator = line.IndexOf(':');
                if (separator == -1)
                {
                    throw new Exception("invalid http header line:" + line);
                }
                string name = line.Substring(0, separator);
                int pos = separator + 1;
                while ((pos < line.Length) && (line[pos] == ' '))
                {
                    pos++;
                }

                string value = line.Substring(pos, line.Length - pos);
                Console.WriteLine("header:{0}:{1}", name, value);
                m_HttpHeadersTable[name] = value;
            }
        }

        /// <summary>
        /// 读取一行;
        /// </summary>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        public string StreamReadLine(Stream inputStream)
        {
            int nextChar;
            string retStr = string.Empty;
            while(true)
            {
                nextChar = inputStream.ReadByte();
                if (nextChar == '\n')
                {
                    break;
                }
                if(nextChar=='\n')
                {
                    continue;
                }
                if(nextChar==-1)
                {
                    continue;
                }

                retStr += Convert.ToChar(nextChar);
            }
            return retStr;
        }
    
        /// <summary>
        /// 返回成功;
        /// </summary>
        public void WriteSuccess()
        {
            m_OutputStreamWriter.WriteLine("HTTP/1.0 200 OK");
            m_OutputStreamWriter.WriteLine("Content-Type: text/html");
            m_OutputStreamWriter.WriteLine("Connection: close");
            m_OutputStreamWriter.WriteLine("");
        }

        /// <summary>
        /// 返回失败;
        /// </summary>
        private void WriteFailure()
        {
            m_OutputStreamWriter.WriteLine("HTTP/1.0 404 File not found");
            m_OutputStreamWriter.WriteLine("Connection: close");
            m_OutputStreamWriter.WriteLine("");
        }
    }
}
