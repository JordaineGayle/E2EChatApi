using E2ECHATAPI.Helpers;
using E2ECHATAPI.Services.UserServices;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E2ECHATAPI.Services.MessageServices
{
    public class RoomService
    {
        readonly IRoomRepository db;
        readonly IUserRepository userdb;
        readonly IHubContext<RoomHub, IRoomHub> hub;

        private RoomService(IRoomRepository db, IUserRepository userdb)
        {
            this.db = db;
            this.userdb = userdb;
            this.hub = RoomClient.Instance.hub;
        }

        /// <summary>
        /// Creates a single instance of the room service
        /// </summary>
        public static readonly Lazy<Task<RoomService>> Instance = new(async () =>
        {
            var db = await RoomRepository.Instance.Value;
            var userdb = await UserRepository.Instance.Value;
            return new(db,userdb);
        }, true);

        public async Task<Room> CreateRoomAsync(RequestContext ctx, CreateRoomRequest request)
        {
            var connectionId = GetConnectionId(ctx);
            var room = new Room(request, ctx.User);
            room = await db.UpsertAsync(room);
            await hub.Groups.AddToGroupAsync(connectionId, room.id, ctx.Cancelled);
            return room;
        }

        public IEnumerable<Room> GetRoomsByUserAsync(RequestContext ctx)
        {
            var rooms = db.GetRoomsByUserId(ctx.User.id);
            return rooms;
        }

        public async Task<Room> JoinRoomAsync(RequestContext ctx, string roomId)
        {
            var connectionId = GetConnectionId(ctx);

            var room = db.Get(roomId);
            if (room == null)
                throw new Exception("unable to locate room.");
            if (room.LimitedReached)
                throw new Exception("room has reached the maximum number of users.");
                
            
            room.JoinRoom(ctx.User.CreateMessageUser());
            room = await db.UpsertAsync(room);

            await hub.Groups.AddToGroupAsync(connectionId, roomId, ctx.Cancelled);
            await hub.Clients.Group(roomId).Joined(ctx.User.CreateMessageUser());

            return room;
        }

        public async Task LeaveRoomAsync(RequestContext ctx, string roomId)
        {
            var connectionId = GetConnectionId(ctx);

            var room = db.Get(roomId);
            if (room == null)
                throw new Exception("unable to locate room.");

            room.LeaveRoom(ctx.User.id);
            room = await db.UpsertAsync(room);

            await hub.Groups.RemoveFromGroupAsync(connectionId, roomId, ctx.Cancelled);
            await hub.Clients.Group(roomId).Left(ctx.User.CreateMessageUser());
        }

        public async Task SendMessageAsync(RequestContext ctx, ChatMessage message)
        {
            var connectionId = GetConnectionId(ctx);

        }


        private string GetConnectionId(RequestContext ctx)
        {
            var exist = RoomHub.connections.TryGetValue(ctx.User.id, out string connectionId);
            if (!exist)
                throw new UnauthorizedAccessException("it appears the user isn't connected to the hub.");
            return connectionId;
        }

    }
}
