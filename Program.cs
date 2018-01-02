using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestOptimus
{
    class Program
    {
        static void Main(string[] args)
        {
            string error;
            if (OptimizeImage(@"C:\Test\ecran.PNG", @"C:\Test\ecran2.webp", OptimusAction.webp, out error))
                MessageBox.Show("OK");
            else
                MessageBox.Show(error);
        }

        private const string licenceKey = "your_key_here";

        public enum OptimusAction { optimize, clean, webp }

        private static bool OptimizeImage(string originalFile, string newFile, OptimusAction action, out string error)
        {
            error = null;
            try
            {
                string url = string.Format("https://api.optimus.io/{0}?{1}", licenceKey, action);
                System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "POST";
                req.UserAgent = "Optimus-API";
                req.Accept = "image/*";
                byte[] bytes = System.IO.File.ReadAllBytes(originalFile);
                req.ContentLength = bytes.Length;
                using (System.IO.Stream stream = req.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
                using (System.IO.Stream input = req.GetResponse().GetResponseStream())
                using (System.IO.Stream output = System.IO.File.OpenWrite(newFile))
                {
                    input.CopyTo(output);
                }
            }
            catch (System.Net.WebException ex)
            {
                error =  ex.Message + (ex.InnerException != null ? "\r\n" + ex.InnerException.Message : "");
                return false;
            }
            catch (Exception ex)
            {
                error = ex.Message + (ex.InnerException != null ? "\r\n" + ex.InnerException.Message : "");
                return false;
            }
            return true;
        }
    }
}
