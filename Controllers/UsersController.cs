using E2ECHATAPI.Services.UserServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E2ECHATAPI.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class UsersController : ParentController
    {

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserRequest request)
        {
            var svc = await UserService.Instance.Value;
            var res = await svc.RegisterUserAsync(request);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var svc = await UserService.Instance.Value;
            var res = svc.Login(request);
            return Ok(res);
        }

        [HttpPut("{fname}")]
        public async Task<IActionResult> UpdateUserFirstName(string fname)
        {
            var svc = await UserService.Instance.Value;
            var res = svc.UpdateFirstNameAsync(RequestContext,fname);
            return Ok(res);
        }

        [HttpPut("{lname}")]
        public async Task<IActionResult> UpdateUserLastName(string lname)
        {
            var svc = await UserService.Instance.Value;
            var res = svc.UpdateLastNameAsync(RequestContext, lname);
            return Ok(res);
        }

        [HttpPut("{avatar}")]
        public async Task<IActionResult> UpdateUserAvatar(string avatar)
        {
            var svc = await UserService.Instance.Value;
            var res = svc.UpdateAvatarAsync(RequestContext, avatar);
            return Ok(res);
        }
    }
}
