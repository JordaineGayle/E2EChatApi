using E2ECHATAPI.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
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
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MinifiedUser))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserRequest request)
        {
            try
            {
                var svc = await UserService.Instance.Value;
                var res = await svc.RegisterUserAsync(request);
                return Ok(res);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MinifiedUser))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                var svc = await UserService.Instance.Value;
                var res = svc.Login(request);
                return Ok(res);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MinifiedUser))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("{fname}")]
        public async Task<IActionResult> UpdateUserFirstName(string fname)
        {
            try
            {
                var svc = await UserService.Instance.Value;
                var res = await svc.UpdateFirstNameAsync(RequestContext, fname);
                return Ok(res);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }

        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MinifiedUser))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("{lname}")]
        public async Task<IActionResult> UpdateUserLastName(string lname)
        {
            try
            {
                var svc = await UserService.Instance.Value;
                var res = await svc.UpdateLastNameAsync(RequestContext, lname);
                return Ok(res);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }

        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MinifiedUser))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("{avatar}")]
        public async Task<IActionResult> UpdateUserAvatar(string avatar)
        {
            try
            {
                var svc = await UserService.Instance.Value;
                var res = svc.UpdateAvatarAsync(RequestContext, avatar);
                return Ok(res);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }

        }
    }
}
