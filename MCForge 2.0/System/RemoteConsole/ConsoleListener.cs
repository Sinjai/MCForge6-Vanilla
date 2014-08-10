using MCForge.Groups;
using MCForge.Utils.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Reflection;

namespace MCForge.Core.RemoteConsole
{
    public class ConsoleListener
    {
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

        public void Start()
        {
            try
            {
                string searchDir = Environment.CurrentDirectory + "/files/gui";

                HttpListener listener = new HttpListener();

                listener.Prefixes.Add("http://localhost:6969/");
                listener.Prefixes.Add("http://localhost:6969/css/");
                listener.Prefixes.Add("http://localhost:6969/js/");
                listener.Prefixes.Add("http://localhost:6969/fonts/");
                listener.Start();
                Console.WriteLine("Listening...");
                string url = "";

                while (true)
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

                            foreach(string kv in keyvalue)
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

                        if(url == "/settings.html")
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
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        public string parseSettings(string file)
        {
            List<SettingNode> ex = ServerSettings.All().Where(e => e.Key != null).ToList();

            foreach (SettingNode node in ex)
            {
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
