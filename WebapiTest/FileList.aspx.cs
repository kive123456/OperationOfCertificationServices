using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services;

namespace WebapiTest
{
    public partial class FileList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ServerFileHelper sf = new ServerFileHelper();
            IEnumerable<string> files = sf.GetAllFile();
            foreach (var file in files)
            {
                this.div.InnerHtml += string.Format("<table><tr><td>{0}</td><td><img src='handler/FileDownload.ashx?filename={1}' alt='图片预览' width='100' height='100' /></td><td><a title='download'>下载</a></td><td><a title='delete'>删除</a></td></tr></table>", file, file);
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var sf = new ServerFileHelper();
            string fullPath = Path.GetFullPath(this.FileUpload1.PostedFile.FileName);
            string[] files = { fullPath };
            var v = sf.UploadFiles(files);
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(v);
            //Response.Write(json);
        }

        [WebMethod]
        public static string Delete(string filename)
        {
            var sf = new ServerFileHelper();
            string str = sf.Delete(filename);
            return str;
        }
    }
}