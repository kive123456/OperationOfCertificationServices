using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebapiTest
{
    public class ServerFileHelper
    {
        private readonly string api = "http://192.168.181.56:31225/api/values";

        public IEnumerable<string> GetAllFile() {
            Uri server = new Uri(api);
            HttpClient httpClient = new HttpClient();

            MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
            
            HttpResponseMessage responseMessage = httpClient.GetAsync(server).Result;

            if (responseMessage.IsSuccessStatusCode)
            {
                IList<string> Files = new List<string>();

                string content = responseMessage.Content.ReadAsStringAsync().Result;

                Files = Newtonsoft.Json.JsonConvert.DeserializeObject<IList<string>>(content);

                if (Files.Count > 0)
                    return Files;
                else
                    return null;


            }
            return null;
        }

        public IEnumerable<HDFile> UploadFiles(params string[] FullFileNames)
        {
            try {
                Uri server = new Uri(api);
                HttpClient httpClient = new HttpClient();

                MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();

                foreach (string fullfilename in FullFileNames)
                {
                    string filename = Path.GetFileName(fullfilename);
                    string filenameWithoutExtension = Path.GetFileNameWithoutExtension(fullfilename);
                    StreamContent streamConent = new StreamContent(new FileStream(fullfilename, FileMode.Open, FileAccess.Read, FileShare.Read));

                    multipartFormDataContent.Add(streamConent, filenameWithoutExtension, filename);
                }

                HttpResponseMessage responseMessage = httpClient.PostAsync(server, multipartFormDataContent).Result;
                if (responseMessage.IsSuccessStatusCode)
                {
                    IList<HDFile> hdFiles = null;

                    string content = responseMessage.Content.ReadAsStringAsync().Result;

                    hdFiles = Newtonsoft.Json.JsonConvert.DeserializeObject<IList<HDFile>>(content);
                    if (hdFiles.Count > 0)
                        return hdFiles;
                    else
                        return null;


                }
                return null;
            } catch (Exception e) {
                Log4Helper.Error("异常" + e);
                throw e;
            }           
        }

        public Stream DownLoad(string ServerFileName)
        {
            Uri server = new Uri(String.Format("{0}?filename={1}", api, ServerFileName));
            HttpClient httpClient = new HttpClient();

            HttpResponseMessage responseMessage = httpClient.GetAsync(server).Result;
            if (responseMessage.IsSuccessStatusCode)
            {
                Stream streamFromService = responseMessage.Content.ReadAsStreamAsync().Result;   
                return streamFromService;
            }
            return null;
        }

        public string Delete(string ServerFileName) {
            Uri server = new Uri(String.Format("{0}?filename={1}", api, ServerFileName));
            HttpClient httpClient = new HttpClient();

            HttpResponseMessage responseMessage = httpClient.DeleteAsync(server).Result;

            if (responseMessage.IsSuccessStatusCode)
            {
                string content = responseMessage.Content.ReadAsStringAsync().Result;
                return content;
            }
            return null;
        }

    }
}
