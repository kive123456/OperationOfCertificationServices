using System.Web;
using System.IO;

namespace WebapiTest.handler
{
    /// <summary>
    /// FileUpload 的摘要说明
    /// </summary>
    public class FileUpload : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.CacheControl = "no-cache";
            string json = string.Empty;
            for (int i = 0; i < context.Request.Files.Count; i++)
            {
                var uploadFile = context.Request.Files[i];
                var sf = new ServerFileHelper();
                string fullPath = Path.GetFullPath(uploadFile.FileName);
                string[] files = { fullPath };
                var v = sf.UploadFiles(files);
                json += Newtonsoft.Json.JsonConvert.SerializeObject(v);
            }
            context.Response.Write(json);
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