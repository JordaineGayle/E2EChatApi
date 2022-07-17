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

        public Room GetRoom(string id) => db.Get(id).EnsureNotNull();

        public Room TryGetRoom(string id) => db.Get(id);

        public async Task<Room> CreateRoomAsync(RequestContext ctx, CreateRoomRequest request)
        {
            var connectionId = GetConnectionId(ctx);
            var room = new Room(request, ctx.User);
            room = await db.UpsertAsync(room);
            await hub.Groups.AddToGroupAsync(connectionId, room.id, ctx.Cancelled);
            return room;
        }

        public IEnumerable<Room> GetRoomsByUser(RequestContext ctx)
        {
            var rooms = db.GetRoomsByUserId(ctx.User.id);
            return rooms;
        }

        public IEnumerable<Room> GetPublicRooms(RequestContext ctx)
        {
            var rooms = db.GetAllRooms()
                .Where(s => !s.Private);
            return rooms;
        }

        public async Task<Room> UpdateTopicAsync(RequestContext ctx, string roomId, string topic)
        {
            var connectionId = GetConnectionId(ctx);
            var room = GetRoom(roomId);
            room.UpdateTopic(ctx.User.id, topic);
            room = await db.UpsertAsync(room);
            await hub.Clients.GroupExcept(room.id, connectionId).RoomTopicUpdated(room);
            return room;
        }

        public async Task<Room> UpdateDescriptionAsync(RequestContext ctx, string roomId, string desc)
        {
            var connectionId = GetConnectionId(ctx);
            var room = GetRoom(roomId);
            room.UpdateDescription(ctx.User.id, desc);
            room = await db.UpsertAsync(room);
            await hub.Clients.GroupExcept(room.id, connectionId).RoomDescriptionUpdated(room);
            return room;
        }

        public async Task<Room> UpdateLimitAsync(RequestContext ctx, string roomId, int? limit)
        {
            var connectionId = GetConnectionId(ctx);
            var room = GetRoom(roomId);
            room.UpdateLimit(ctx.User.id, limit);
            room = await db.UpsertAsync(room);
            await hub.Clients.GroupExcept(room.id, connectionId).RoomUpdated(room);
            return room;
        }

        public async Task<Room> JoinRoomAsync(RequestContext ctx, string roomId)
        {
            var connectionId = GetConnectionId(ctx);

            var room = GetRoom(roomId);

            if (room.LimitedReached)
                throw new Exception("room has reached the maximum number of users.");
            
            room.JoinRoom(ctx.User.CreateMessageUser());
            room = await db.UpsertAsync(room);

            await hub.Groups.AddToGroupAsync(connectionId, roomId, ctx.Cancelled);
            await hub.Clients.GroupExcept(room.id, connectionId).Joined(ctx.MessageUser);

            return room;
        }

        public async Task LeaveRoomAsync(RequestContext ctx, string roomId)
        {
            var connectionId = GetConnectionId(ctx);

            var room = GetRoom(roomId);

            room.LeaveRoom(ctx.User.id);
            room = await db.UpsertAsync(room);

            await hub.Groups.RemoveFromGroupAsync(connectionId, roomId, ctx.Cancelled);
            await hub.Clients.GroupExcept(room.id, connectionId).Left(ctx.MessageUser);
        }

        public async Task<MessageBody> SendMessageAsync(RequestContext ctx, string roomId, ChatMessage message)
        {
            var connectionId = GetConnectionId(ctx);
            var room = db.Get(roomId);
            User to = null;
            if(!(message?.Receiver?.IsNullOrEmptyWhitespace()??false))
            {
                to = userdb.Get(message.Receiver);
            }
            var msgBody = MessageBody.CreateMessage(message, ctx.MessageUser, to?.CreateMessageUser());
            room.AddMessage(msgBody);
            room = await db.UpsertAsync(room);
            await hub.Clients.GroupExcept(room.id,connectionId).MessageReceived(msgBody);
            return msgBody;
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
