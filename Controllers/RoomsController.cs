using E2ECHATAPI.Services.MessageServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E2ECHATAPI.Controllers
{
    [ApiController]
    public class RoomsController : ParentController
    {
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Room))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("api/rooms/{id}/GetRoom")]
        [HttpGet]
        public async Task<IActionResult> GetRoom(string id)
        {
            var svc = await RoomService.Instance.Value;
            var res = svc.GetRoom(id);
            return Ok(res);
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Room>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("api/rooms/GetMyRooms")]
        [HttpGet]
        public async Task<IActionResult> GetMyRooms()
        {
            var svc = await RoomService.Instance.Value;
            var res = svc.GetRoomsByUser(RequestContext);
            return Ok(res);
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Room>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("api/rooms/GetOpenRooms")]
        [HttpGet]
        public async Task<IActionResult> GetOpenRooms()
        {
            var svc = await RoomService.Instance.Value;
            var res = svc.GetPublicRooms(RequestContext);
            return Ok(res);
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Room))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("api/rooms/CreateRoom")]
        [HttpPost]
        public async Task<IActionResult> CreateRoom(CreateRoomRequest request)
        {
            var svc = await RoomService.Instance.Value;
            var res = await svc.CreateRoomAsync(RequestContext,request);
            return Ok(res);
        }


        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Room))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("api/rooms/{id}/UpdateTopic/{topic}")]
        [HttpPut]
        public async Task<IActionResult> CreateRoom(string id, string topic)
        {
            var svc = await RoomService.Instance.Value;
            var res = await svc.UpdateTopicAsync(RequestContext,id,topic);
            return Ok(res);
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Room))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("api/rooms/{id}/UpdateDescription/{desc}")]
        [HttpPut]
        public async Task<IActionResult> UpdateDescription(string id, string desc)
        {
            var svc = await RoomService.Instance.Value;
            var res = await svc.UpdateDescriptionAsync(RequestContext, id, desc);
            return Ok(res);
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Room))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("api/rooms/{id}/UpdateLimit/{limit?}")]
        [HttpPut]
        public async Task<IActionResult> UpdateLimit(string id, int? limit)
        {
            var svc = await RoomService.Instance.Value;
            var res = await svc.UpdateLimitAsync(RequestContext, id, limit);
            return Ok(res);
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Room))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("api/rooms/{id}/JoinRoom")]
        [HttpPut]
        public async Task<IActionResult> JoinRoom(string id)
        {
            var svc = await RoomService.Instance.Value;
            var res = await svc.JoinRoomAsync(RequestContext, id);
            return Ok(res);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("api/rooms/{id}/LeaveRoom")]
        [HttpPut]
        public async Task<IActionResult> LeaveRoom(string id)
        {
            var svc = await RoomService.Instance.Value;
            await svc.LeaveRoomAsync(RequestContext, id);
            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MessageBody))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("api/rooms/{id}/SendMessage")]
        [HttpPut]
        public async Task<IActionResult> SendMessage(string id, ChatMessage message)
        {
            var svc = await RoomService.Instance.Value;
            var res = await svc.SendMessageAsync(RequestContext, id, message);
            return Ok(res);
        }

    }
}
