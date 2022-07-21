using E2ECHATAPI.Services.UserServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace E2ECHATAPI.Services.MessageServices
{
    public interface IChatHub
    {
        public Task MessageReceived(string conversationId, string message);
        public Task ConversationStarted(ConversationResponse response);
        public Task Connected(Contact contact);
        public Task Disconnected(Contact contact);
        public Task RetreivedContactOnline(IReadOnlyList<Contact> contacts);
        public Task IsTyping(string conversationId, string message);
    }

    [Authorize(Policy = "ChatHubAuthPolicy", AuthenticationSchemes = "ChatHubAuthScheme")]
    public class ChatHub : Hub<IChatHub>
    {
        readonly ConcurrentDictionary<string, User> users = new();
        readonly ConcurrentDictionary<string, Conversation> conversations = new();
        readonly ConcurrentDictionary<string, IList<ChatMessage>> queued = new();

        public override async Task OnConnectedAsync()
        {
            var svc = await UserService.Instance.Value;

            var user = svc.GetUser(Context.UserIdentifier);

            users.AddOrUpdate(user.id, user, (key, o) => user);

            await Clients.All.Connected(user.CreateContact());

            queued.TryRemove(user.id, out IList<ChatMessage> messages);

            if(messages != null)
            {
                foreach(var message in messages)
                {
                    await Clients.Caller.MessageReceived(message.ConversationId,message.Message);
                }
            }

        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            users.TryRemove(Context.UserIdentifier, out User user);
            if (user != null)
                await Clients.All.Disconnected(user.CreateContact(false,DateTimeOffset.UtcNow));
        }

        public async Task StartConversation(string userId)
        {
            var svc = await UserService.Instance.Value;

            var user = svc.GetUser(userId);

            var publicKey = user.PublicKey;

            var key = Conversation.GenerateConversationKey(Context.UserIdentifier, userId);

            conversations.TryGetValue(key, out Conversation conversation);

            if(conversation == null)
            {
                var initiator = svc.GetUser(Context.UserIdentifier);
                conversation = Conversation.CreateConsersation(initiator.CreateMessageUser(), user.CreateMessageUser());
            }

            conversations.AddOrUpdate(key, conversation, (key, old) => conversation);

            await Clients.Caller.ConversationStarted(new(publicKey,key));
        }

        public async Task SendMessage(string userId, string message)
        {
            var svc = await UserService.Instance.Value;

            var user = svc.GetUser(userId);

            var chat = new ChatMessage()
            {
                Sender = Context.UserIdentifier,
                Receiver = user.id,
                Message = message,
                Date = DateTimeOffset.UtcNow
            };

            conversations.TryGetValue(chat.ConversationId, out Conversation conversation);

            conversation?.AddMessage(chat);

            users.TryGetValue(userId, out User receiver);

            if(receiver == null)
            {
                queued.TryGetValue(userId, out IList<ChatMessage> messages);

                if (messages == null)
                    messages = new List<ChatMessage>();
                messages.Add(chat);

                queued.AddOrUpdate(userId, messages, (key, o) => messages);
            }

            await Clients.User(userId).MessageReceived(chat.ConversationId,message);
        }

        public async Task RetrieveContactsOnline()
        {
            await Clients.Caller.RetreivedContactOnline(users.Values.Select(x => x.CreateContact(true)).ToArray());
        }

        public async Task Typing(string conversationId, string userId)
        {
            await Clients.User(userId).IsTyping(conversationId, $"{Context.User.Identity.Name} is typing...");
        }

    }


    public class ChatHubAuthSchemeOptions
        : AuthenticationSchemeOptions
    { }

    public class ChatHubAuthHandler
        : AuthenticationHandler<ChatHubAuthSchemeOptions>
    {
        public ChatHubAuthHandler(
            IOptionsMonitor<ChatHubAuthSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            Request.Query.TryGetValue("apiKey", out StringValues token);

            var svc = UserServices.UserService.Instance.Value.Result;

            var user = svc.TryGetUser(token.ToString());

            if (user != null)
            {
                var claims = new[] {
                    new Claim(ClaimTypes.NameIdentifier, user.id),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.FullName) };

                var claimsIdentity = new ClaimsIdentity(claims,
                            nameof(ChatHubAuthHandler));

                var ticket = new AuthenticationTicket(
                    new ClaimsPrincipal(claimsIdentity), this.Scheme.Name);

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            else
            {
                return Task.FromResult(AuthenticateResult.Fail("failed to retrieve user information"));
            }
        }

    }
}
