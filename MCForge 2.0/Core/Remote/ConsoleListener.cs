﻿/*
    Copyright 2014 UclCommander
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    http://www.opensource.org/licenses/ecl2.php
    http://www.gnu.org/licenses/gpl-3.0.html
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
*/
using MCForge.Groups;
using MCForge.Utils;
using MCForge.Utils.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;

namespace MCForge.Core.Remote
{
    public class ConsoleListener
    {
        public int Port
        {
            get { return ServerSettings.GetSettingInt("Remote-Port"); }
            set { ServerSettings.SetSetting("Remote-Port", value.ToString()); }
        }

        public string URL
        {
            get { return String.Format("http://localhost:{0}", this.Port); }
        }

        public string XID { get; internal set;}

        private Thread trd;
        private HttpListener listener;
        private HttpListenerContext context;
        private HttpListenerRequest request;
        private HttpListenerResponse response;

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

        /// <summary>
        /// 
        /// </summary>
        public ConsoleListener()
        {
            using (var numberGen = new RNGCryptoServiceProvider())
            {
                var data = new byte[20];
                var data2 = new byte[20];
                numberGen.GetBytes(data);
                numberGen.GetBytes(data2);

                using(MD5 md5 = new MD5CryptoServiceProvider())
                {
                    string str = ServerSettings.Salt2 + Convert.ToBase64String(data) + Convert.ToBase64String(data2) + ServerSettings.Salt;
                    byte[] buffer = new byte[str.Length * 2];
                    Encoding.Unicode.GetEncoder().GetBytes(str.ToCharArray(), 0, str.Length, buffer, 0, true);
                    byte[] result = md5.ComputeHash(buffer);

                    StringBuilder sb = new StringBuilder();
                    
                    for (int i = 0; i < result.Length; i++)
                        sb.Append(result[i].ToString("X2"));

                    this.XID = sb.ToString(); 
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            this.trd = new Thread(new ThreadStart(delegate
            {
                try
                {
                    string searchDir = Environment.CurrentDirectory + "/files/gui";

                    this.listener = new HttpListener();

                    this.listener.Prefixes.Add(String.Format("{0}/", this.URL));
                    this.listener.Prefixes.Add(String.Format("{0}/css/", this.URL));
                    this.listener.Prefixes.Add(String.Format("{0}/js/", this.URL));
                    this.listener.Prefixes.Add(String.Format("{0}/fonts/", this.URL));
                    this.listener.Start();
                    Console.WriteLine("Listening...");
                    string url = "";

                    while (!Server.ShuttingDown)
                    {
                        this.context = this.listener.GetContext();
                        this.request = this.context.Request;
                        this.response = this.context.Response;
                        Dictionary<string, string> post = new Dictionary<string, string> { };

                        try
                        {
                            if (this.request.HttpMethod == "POST")
                            {
                                string postData;

                                using (StreamReader reader = new StreamReader(this.request.InputStream, this.request.ContentEncoding))
                                    postData = reader.ReadToEnd();

                                string[] keyvalue = postData.Split('&');

                                foreach (string kv in keyvalue)
                                {
                                    string[] setting = kv.Split('=');
                                    string safe = HttpUtility.UrlDecode(setting[1]);

                                    if (post.ContainsKey(setting[0]))
                                        post[setting[0]] = safe;
                                    else
                                        post.Add(setting[0], safe);
                                }
                            }

                            url = this.request.Url.LocalPath;
                            Console.WriteLine(url);
                            string file = searchDir + url;

                            //ignore folders that do nothing
                            if (this.request.Url.Segments[1] != "css/" &&
                                this.request.Url.Segments[1] != "js/" && 
                                this.request.Url.Segments[1] != "fonts/")
                            {
                                //WARNING: HACKR INCOMIN
                                if (this.request.QueryString["XID"] != this.XID || (post.ContainsKey("XID") && post["XID"] != this.XID))
                                {
                                    Logger.Log("WARNING: Someone tried to access Remote Console page '" + url +  "' without the correct XID!", LogType.Warning);
                                    this.SendData("Can't let you do that, Starfox.");
                                    continue; ;
                                }
                            }

                            if(post.Count() > 0)
                            {
                                foreach(var data in post)
                                    if(data.Key != "XID")
                                        ServerSettings.SetSetting(data.Key, null, (data.Value == "on" ? "true" : (data.Value == "off" ? "false" : data.Value)));
                                
                                //force write to file
                                ServerSettings.Save();
                            }

                            byte[] buffer;
                            string filename = url.Split('/').Last().TrimStart('/').ToLower().Replace(".", "_").Replace("-", "_");
                            byte[] streamData = MCForge.HtmlData.HtmlData.GetResource(filename);

                            if (url == "/settings.html")
                            {
                                string data = Encoding.UTF8.GetString(streamData);
                                data = this.parseSettings(data);
                                buffer = Encoding.UTF8.GetBytes(data);
                            }
                            else
                                buffer = streamData;

                            FileInfo fi = new FileInfo(file);

                            this.SendData(buffer, this.mimeTypes[fi.Extension]);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="mime"></param>
        public void SendData(string data, string mime = "text/plain")
        {
            this.SendData(Encoding.UTF8.GetBytes(data), mime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="mime"></param>
        public void SendData(byte[] buffer, string mime)
        {
            Stream output = this.response.OutputStream;

            this.response.ContentLength64 = buffer.Length;
            this.response.AddHeader("Content-type", mime);
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            this.listener.Stop();
            this.trd.Abort();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
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
            file = file.Replace("{{{XID}}}", this.XID);
            return file;
        }
    }

}
