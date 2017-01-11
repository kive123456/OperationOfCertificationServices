using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using System.Drawing;

namespace OperationOfCertificationServices.Controllers
{
    public class ValuesController : ApiController
    {
        private const string UploadFolder = "uploads";

        private const string ThumbnailFolder = "Thumbnail";
        public IEnumerable<string> Get()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(HostingEnvironment.MapPath("~/App_Data/" + UploadFolder));
            string[] files = new string[directoryInfo.GetFiles().Length];
            for (int i = 0; i < directoryInfo.GetFiles().Length; i++)
            {
                files[i] = directoryInfo.GetFiles()[i].Name;
            }
            return files;
        }

        public HttpResponseMessage Get(string fileName)
        {
            try
            {
                //程序运行代码
                HttpResponseMessage result = null;

                DirectoryInfo directoryInfo = new DirectoryInfo(HostingEnvironment.MapPath("~/App_Data/" + UploadFolder));
                FileInfo foundFileInfo = directoryInfo.GetFiles().Where(x => x.Name == fileName).FirstOrDefault();
                if (foundFileInfo != null)
                {
                    FileStream fs = new FileStream(foundFileInfo.FullName, FileMode.Open);

                    result = new HttpResponseMessage(HttpStatusCode.OK);
                    result.Content = new StreamContent(fs);
                    result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    result.Content.Headers.ContentDisposition.FileName = foundFileInfo.Name;
                }
                else
                {
                    result = new HttpResponseMessage(HttpStatusCode.NotFound);
                }

                return result;
            }
            catch (IOException ex)
            {
                //异常信息写入日志
                //程序进入错误页
                Log4Helper.Error("IO异常" + ex);
            }
            catch (Exception e)
            {
                //异常信息写入日志
                //程序进入错误页
                Log4Helper.Error("异常" + e);
            }
            finally
            {
                //关闭操作
            }
            return null;
        }

        public Task<IQueryable<HDFile>> Post()
        {
            try
            {
                string uploadFolderPath = HostingEnvironment.MapPath("~/App_Data/" + UploadFolder);

                //如果路径不存在，创建路径
                if (!Directory.Exists(uploadFolderPath))
                    Directory.CreateDirectory(uploadFolderPath);

                if (Request.Content.IsMimeMultipartContent()) //If the request is correct, the binary data will be extracted from content and IIS stores files in specified location.
                {
                    var streamProvider = new WithExtensionMultipartFormDataStreamProvider(uploadFolderPath);
                    var task = Request.Content.ReadAsMultipartAsync(streamProvider).ContinueWith<IQueryable<HDFile>>(t =>
                    {
                        if (t.IsFaulted || t.IsCanceled)
                        {
                            throw new HttpResponseException(HttpStatusCode.InternalServerError);
                        }

                        var fileInfo = streamProvider.FileData.Select(i =>
                        {
                            var info = new FileInfo(i.LocalFileName);

                            PicDeal.MakeThumbnail(info.FullName, 100, 100, ThumbnailMod.W);

                            return new HDFile(info.Name, Request.RequestUri.AbsoluteUri + "?filename=" + info.Name, (info.Length / 1024).ToString());
                        });

                        return fileInfo.AsQueryable();
                    });
                    Log4Helper.Info(task.ToString());
                    return task;
                }
                else
                {
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotAcceptable, ""));
                }
            }
            catch (IOException ex)
            {
                //异常信息写入日志
                //程序进入错误页
                Log4Helper.Error("IO异常" + ex);
            }
            catch (Exception e)
            {
                Log4Helper.Error("异常" + e);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, e.Message));
            }
            finally
            {
                //关闭操作
            }
            return null;
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {

        }

        // DELETE api/values/5
        public string Delete(string filename)
        {
            try
            {
                string filepath = HostingEnvironment.MapPath(@"~/App_Data/" + UploadFolder + "/" + filename);
                if (File.Exists(filepath))
                {
                    File.Delete(filepath);
                    return "删除成功";
                }
                else
                {
                    return "文件不存在";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public class HDFile
        {
            public HDFile(string name, string url, string size)
            {
                Name = name;
                Url = url;
                Size = size;
            }

            public string Name { get; set; }

            public string Url { get; set; }

            public string Size { get; set; }
        }
    }
}