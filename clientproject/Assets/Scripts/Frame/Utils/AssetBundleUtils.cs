using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GameFrame
{
    public class AssetBundleUtils
    {
        public static string GetMD5(string path)
        {
            try
            {
                Stream st = new FileStream(path, FileMode.Open, FileAccess.Read);
                MD5 md5 = new MD5CryptoServiceProvider();
                StringBuilder sb = new StringBuilder();
                var temp = md5.ComputeHash(st);
                foreach (var item in temp)
                {
                    sb.Append(item.ToString("x2"));
                }
                st.Dispose();
                return sb.ToString();
            }
            catch (Exception ex)
            {
                FrameDebug.LogError(ex.ToString());
                return "";
            }
        }
    }
}
