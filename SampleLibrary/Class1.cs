using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using System.Web.Mvc;

namespace TestSourceLibrary
{
    [Route("test")]
    public partial class Class1 : Controller
    {
        [HttpPost("search")]
        [Authorize]
        public IEnumerable<int> Test()
        {
            return null;
        }

        [HttpPost("search")]
        public Task<IEnumerable<int>> Test1()
        {
            return null;
        }
    }
}
