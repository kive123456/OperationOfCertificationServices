using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;


namespace OperationOfCertificationServices.Controllers
{
    public class TokenController : ApiController
    {
        // GET api/values
        public string Get()
        {
            return Guid.NewGuid().ToString();
        }

        public bool IsToken(string token) {
            return false;
        }
    }
}
