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
        public string id { get; private set; }
        public string Message { get; private set; }
        public bool Read { get; private set; }
        public bool IsDeleted { get; private set; }
        public DateTimeOffset LastModified { get; private set; }
        public DateTimeOffset DateCreated { get; private set; }
        public HashSet<string> Reactions { get; private set; } = new();
        public MessageUser From {get; private set; }
        public MessageUser To {get; private set; }
    }

    /// <summary>
    /// Chat message to facilitate communication among users
    /// </summary>
    public record ChatMessage
    {
        public string RoomToken { get; set; }
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
            Contracts.EnsureNotNullOrEmpty(message.RoomToken, "a room token is required.");
            Contracts.EnsureNotNullOrEmpty(message.Sender, "sender is required.");
            Contracts.EnsureNotNullOrEmpty(message.Receiver, "receiver is required.");
            Contracts.EnsureNotNullOrEmpty(message.Receiver, "receiver is required.");
            Contracts.EnsureNotNullOrEmpty(message.Message, "message is required.");
            this.RoomToken = message.RoomToken;
            this.Message = message.Message;
            this.Sender = message.Sender;
            this.Receiver = message.Receiver;
        }
    }

    /// <summary>
    /// Request to edit a message
    /// </summary>
    public record EditMessageRequest
    {
        public string UserId { get; set; }
        public string MessageId { get; set; }
        public string Message { get; set; }

        public EditMessageRequest() { }

        public EditMessageRequest(string userId, EditMessageRequest request)
        {
            Contracts.RequiresNotNull(request, "edit message request is required.");
            Contracts.EnsureNotNullOrEmpty(userId, "user id is required for this request.");
            Contracts.EnsureNotNullOrEmpty(request.MessageId, "message id is required.");
            Contracts.EnsureNotNullOrEmpty(request.Message, "actual message is needed.");
            this.MessageId = request.MessageId;
            this.Message = request.Message;
        }
    }

    /// <summary>
    /// Request to delete a message
    /// </summary>
    public record DeleteMessageRequest(string UserId, string MessageId);
}
