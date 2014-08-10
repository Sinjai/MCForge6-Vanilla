using MCForge.HtmlData.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MCForge.HtmlData
{
    public static class HtmlData
    {

        public static byte[] GetResource(string name)
        {
            object obj = Resources.ResourceManager.GetObject(name);

            if (obj.GetType() == typeof(string))
                return Encoding.UTF8.GetBytes((string)obj);

            return ((byte[])(obj));
        }
    }
}
