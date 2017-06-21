using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Shop.API.Controllers
{
    public class TestController : ApiController
    {
        [HttpGet]
        public Task<int> Echo(int echovalue)
        {
            return Task.FromResult(echovalue);
        }
    }
}
