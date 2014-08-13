using MCForge.Groups;
using MCForge.Utils.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;

namespace MCForge.Core.RemoteConsole
{
    public class ConsoleListener
    {
        private static ConsoleListener self;
        private Thread trd;
        private HttpListener listener;
        private int port = 6969;
        private Dictionary<string, string> mimeTypes = new Dictionary<string, string>()
        { 
            {".html",   "text/html"},
            {".css",    "text/css"},
            {".js",     "application/x-javascript"},
            {".ttf",    "application/x-font-ttf"},
            {".woff",   "application/font-woff"},
            {".svg",    "image/svg+xml"},
            {".eot",    "application/vnd.ms-fontobject"},
            {".otf",    "application/x-font-otf"}
        };

        public static ConsoleListener Self { get{ return self; } }

        public void Start()
        {
            self = this;

            this.trd = new Thread(new ThreadStart(delegate
            {
                try
                {
                    string searchDir = Environment.CurrentDirectory + "/files/gui";

                    this.listener = new HttpListener();

                    this.listener.Prefixes.Add("http://localhost:6969/");
                    this.listener.Prefixes.Add("http://localhost:6969/css/");
                    this.listener.Prefixes.Add("http://localhost:6969/js/");
                    this.listener.Prefixes.Add("http://localhost:6969/fonts/");
                    this.listener.Start();
                    Console.WriteLine("Listening...");
                    string url = "";

                    while (!Server.ShuttingDown)
                    {
                        HttpListenerContext context = listener.GetContext();
                        HttpListenerRequest request = context.Request;
                        HttpListenerResponse response = context.Response;

                        try
                        {
                            url = request.RawUrl;
                            Console.WriteLine(url);
                            string file = searchDir + url;

                            byte[] buffer;

                            if (request.HttpMethod == "POST")
                            {
                                string postData;

                                using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
                                {
                                    postData = reader.ReadToEnd();
                                }

                                string[] keyvalue = postData.Split('&');

                                foreach (string kv in keyvalue)
                                {
                                    string[] setting = kv.Split('=');
                                    string safe = HttpUtility.UrlDecode(setting[1]);

                                    ServerSettings.SetSetting(setting[0], null, (safe == "on" ? "true" : (safe == "off" ? "false" : safe)));
                                }
                                //force write to file
                                ServerSettings.Save();
                            }

                            string filename = url.Split('/').Last().TrimStart('/').ToLower().Replace(".", "_").Replace("-", "_");
                            byte[] streamData = MCForge.HtmlData.HtmlData.GetResource(filename);

                            if (url == "/settings.html")
                            {
                                string data = Encoding.UTF8.GetString(streamData);
                                data = this.parseSettings(data);
                                buffer = System.Text.Encoding.UTF8.GetBytes(data);
                            }
                            else
                                buffer = streamData;


                            FileInfo fi = new FileInfo(file);

                            response.ContentLength64 = buffer.Length;
                            response.AddHeader("Content-type", this.mimeTypes[fi.Extension]);
                            System.IO.Stream output = response.OutputStream;
                            output.Write(buffer, 0, buffer.Length);
                            output.Close();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine("FILE" + request.RawUrl);
                        }
                    }

                    listener.Stop();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }));

            trd.Start();
        }


        public void Stop()
        {
            this.listener.Stop();
            this.trd.Abort();
        }

        public string parseSettings(string file)
        {
            List<SettingNode> ex = ServerSettings.All().Where(e => e.Key != null).ToList();

            foreach (SettingNode node in ex)
            {
                Console.WriteLine("ASSIGN: " + node.Key + " TO " + node.Value);
                file = file.Replace("{{{" + node.Key + "}}}", node.Value);
            }

            List<PlayerGroup> groups = PlayerGroup.Groups;

            string json = "[";

            foreach(PlayerGroup g in groups)
            {
                json += "{";
                json += "\"name\":\"" + g.Name + "\",";
                json += "\"permission\":\"" + g.Permission + "\",";
                json += "\"color\":\"" + g.Color + "\",";
                json += "\"maxblockchanges\":\"" + g.MaxBlockChange + "\",";
                json += "\"file\":\"" + g.File + "\"";
                json += "},";
            }

            json = json.TrimEnd(',');
            json += "]";

            file = file.Replace("{{{GROUP_JSON}}}", json);

            return file;
        }
    }

}
