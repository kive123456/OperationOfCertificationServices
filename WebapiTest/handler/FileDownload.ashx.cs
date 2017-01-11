using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace WebapiTest.handler
{
    /// <summary>
    /// FileDownload 的摘要说明
    /// </summary>
    public class FileDownload : IHttpHandler
    {

        public void ProcessRequest(HttpContext Context)
        {
            var sf = new ServerFileHelper();
            string filename = Context.Request["filename"];
            Stream stream = sf.DownLoad(filename);

            //Stream streama = new MemoryStream();
            //System.Drawing.Bitmap a = new Bitmap(stream);
            //System.Drawing.Image image = a.GetThumbnailImage(100, 75, () => { return false; }, IntPtr.Zero);
            //image.Save(streama, System.Drawing.Imaging.ImageFormat.Jpeg);

            //byte[] bytes = new byte[streama.Length];
            //streama.Read(bytes, 0, bytes.Length);
            //streama.Seek(0, SeekOrigin.Begin);

            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);

            Context.Response.ContentType = "application/octet-stream";
            Context.Response.AppendHeader("Content-Disposition", "attachment;filename=" + filename);

            Context.Response.BinaryWrite(bytes);
            Context.Response.End();
            Context.Response.Close();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}