using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace WebapiTest
{
    public partial class Index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var sf = new ServerFileHelper();
            string fullPath = Path.GetFullPath(this.FileUpload1.PostedFile.FileName);
            string[] files = { fullPath };
            var v = sf.UploadFiles(files);
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(v);
            Response.Write(json);
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            Response.Write(new TokenHelper().GetToken());
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            var sf = new ServerFileHelper();
            Stream stream = sf.DownLoad(this.TextBox1.Text);
            
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);

            Context.Response.ContentType = "application/octet-stream";
            Context.Response.AppendHeader("Content-Disposition", "attachment;filename=" + this.TextBox1.Text);

            Context.Response.BinaryWrite(bytes);
            Context.Response.End();
            Context.Response.Close();
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            var sf = new ServerFileHelper();
            string str = sf.Delete(this.TextBox2.Text);
            Response.Write(str);
        }
    }
}
