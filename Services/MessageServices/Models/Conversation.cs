using E2ECHATAPI.Helpers;
using E2ECHATAPI.Services.UserServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E2ECHATAPI.Services.MessageServices
{
    public record Conversation
    {
        public string id { get; init; }

        public IList<MessageUser> Participants { get; init; } = new List<MessageUser>();

        public IList<ChatMessage> Messages { get; private set; } = new List<ChatMessage>();


        public void AddMessage(ChatMessage message)
        {
            Contracts.RequiresNotNull(message, "message is required.");
            Messages.Add(message);
        }

        public static Conversation CreateConsersation(MessageUser user1, MessageUser user2)
        {
            Contracts.RequiresNotNull(user1, "user 1 is required.");
            Contracts.RequiresNotNull(user2, "user 2 is required.");
            return new Conversation 
            {
                id = GenerateConversationKey(user1.id,user2.id),
                Participants = new List<MessageUser>() { user1, user2 }
            };
        }

        public static string GenerateConversationKey(string id1, string id2)
        {
            var ids = $"{id1}{id2}".ToUpper().ToCharArray().OrderBy(x => x);
            return string.Join("", ids);
        }

    }
}
