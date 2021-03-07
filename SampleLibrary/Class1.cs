using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace TestSourceLibrary
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    sealed class MyRouteAttribute : RouteAttribute
    {
        public MyRouteAttribute(string template) : base(template)
        {
        }
    }

    [MyRoute("test")]
    public partial class Class1 : Controller
    {
        [HttpPost("search")]
        [Authorize]
        public partial IEnumerable<int> Test()
        {
            return new[] { 0 };
        }

        [HttpPost("search")]
        public partial Task<IEnumerable<int>> Test1()
        {
            return null;
        }
    }

    public static class Program
    {
        public static void Main()
        {

        }
    }
}