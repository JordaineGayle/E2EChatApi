using E2ECHATAPI.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E2ECHATAPI.Controllers
{
    [ApiController]
    public class ParentController : ControllerBase
    {
        public RequestContext RequestContext => this.GetRequestContext();
    }
}
