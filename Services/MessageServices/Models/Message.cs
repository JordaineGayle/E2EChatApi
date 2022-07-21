using E2ECHATAPI.Helpers;
using E2ECHATAPI.Services.UserServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E2ECHATAPI.Services.MessageServices
{
    /// <summary>
    /// Domain message entity
    /// </summary>
    public record MessageBody
    {
        public string id { get; init; }
        public string Message { get; init; }
        public bool Read { get; init; }
        public DateTimeOffset DateCreated { get; init; }
        public MessageUser From {get; init; }
        public MessageUser To {get; init; }


        public static MessageBody CreateMessage(ChatMessage chat, MessageUser from, MessageUser to = null)
        {
            Contracts.RequiresNotNull(chat, "chat message is required");
            Contracts.RequiresNotNull(from, "sender is required");
            var message = new MessageBody 
            {
                id = Guid.NewGuid().ToString("N"),
                Message = chat.Message,
                Read = false,
                DateCreated = DateTimeOffset.UtcNow,
                From = from,
                To = to
            };
            return message;
        }

        public MessageBody ReadMessage()
        {
            return this with { Read = true };
        }

    }

    /// <summary>
    /// Chat message to facilitate communication among users
    /// </summary>
    public record ChatMessage
    {
        public string ConversationId => Conversation.GenerateConversationKey(Sender, Receiver);
        public string Message { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }

        /// <summary>
        /// Default constrcutor
        /// </summary>
        public ChatMessage() { }

        /// <summary>
        /// Primary constructor, used to create a new instance of the chat message object.
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public ChatMessage(ChatMessage message)
        {
            Contracts.RequiresNotNull(message, "chat message is required.");
            Contracts.EnsureNotNullOrEmpty(message.Message, "message is required.");
            Contracts.EnsureNotNullOrEmpty(message.Receiver, "receiver is required.");
            this.Message = message.Message;
            this.Receiver = message.Receiver;
        }
    }

    public record ConversationResponse(string PublicKey, string ConversationId);

}
