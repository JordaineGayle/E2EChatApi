using E2ECHATAPI.Services.UserServices;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace E2ECHATAPI.Services.MessageServices
{
    public interface IRoomHub
    {
        Task MessageReceived(MessageBody message);
        Task MessageEdited(MessageBody message);
        Task MessageDeleted(MessageBody message);
        Task RoomTopicUpdated(Room room);
        Task RoomDescriptionUpdated(Room room);
        Task RoomUpdated(Room room);
        Task Joined(MessageUser user);
        Task Left(MessageUser user);
    }

    public class RoomHub : Hub<IRoomHub>
    {
        public static readonly ConcurrentDictionary<string, string> connections = new();

        public override async Task OnConnectedAsync()
        {
            //there is a better way to do this, due to time alotted this approach was taken
            var httpContext = this.Context.GetHttpContext();
            var apiKey = WebUtility.UrlDecode(httpContext.Request.Query["ApiKey"]);
            var svc = await UserService.Instance.Value;
            try
            {
                var user = svc.GetUser(apiKey);
                connections.AddOrUpdate(user.id, Context.ConnectionId, (id, o) => Context.ConnectionId);
            }
            catch(Exception ex)
            {
                throw new HubException(ex.Message);
            }

        }
    }


    public class RoomClient
    {
        public readonly IHubContext<RoomHub, IRoomHub> hub;

        public RoomClient(IHubContext<RoomHub, IRoomHub> hub)
        {
            this.hub = hub;
        }

        public static RoomClient Instance;
    }
}
