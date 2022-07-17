using E2ECHATAPI.Helpers;
using E2ECHATAPI.Services.UserServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E2ECHATAPI.Services.MessageServices
{
    /// <summary>
    /// Request to create a new room
    /// </summary>
    public record CreateRoomRequest
    {
        public string Topic { get; set; }
        public string Description { get; set; }
        public int? Limit { get; set; }
        public bool ReadReceipt { get; set; }
        public bool Reactions { get;  set; }
    }

    /// <summary>
    /// Actual chat room
    /// </summary>
    public record Room
    {
        public string id { get; private set; }
        public string OwnerId { get; private set; }
        public string Topic { get; private set; }
        public string Description { get; set; }
        public bool LimitedReached => !RoomConfiguration.IsUnlimited ? Users.Count == RoomConfiguration.Limit : false;
        public RoomConfiguration RoomConfiguration { get; private set; }
        public IList<MessageUser> Users { get; private set; } = new List<MessageUser>() { };
        public IList<MessageBody> Messages { get; private set; } = new List<MessageBody>() { };
        public DateTimeOffset LastModified { get; private set; }
        public DateTimeOffset DateCreated { get; private set; }

        /// <summary>
        /// Default room constructor
        /// </summary>
        public Room() { }

        /// <summary>
        /// Creates a new room with the given params
        /// </summary>
        /// <param name="request"></param>
        /// <param name="owner"></param>
        public Room(CreateRoomRequest request, User owner)
        {
            Contracts.RequiresNotNull(request, "create room request is needed for this operation.");
            Contracts.RequiresNotNull(owner, "owner for the group is needed for this request.");
            this.id = Guid.NewGuid().ToString("N");
            this.OwnerId = owner.id;
            this.Topic = request.Topic;
            this.Description = request.Description;
            this.RoomConfiguration = new(request.Limit, request.ReadReceipt, request.Reactions);
            this.Users.Add(owner.CreateMessageUser(true));
            this.LastModified = DateTimeOffset.UtcNow;
            this.DateCreated = DateTimeOffset.UtcNow;
        }


        /// <summary>
        /// Gets a single message given the user id and the message id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public MessageBody GetMessage(string id, string userId)
            => Messages.FirstOrDefault(x => x.id.EqualsIgnoreCase(id) 
            && x.From.id.EqualsIgnoreCase(userId))
            .EnsureNotNull();

        /// <summary>
        /// Updates the topic of the room
        /// </summary>
        /// <param name="topic"></param>
        public void UpdateTopic(string topic)
        {
            Contracts.EnsureNotNullOrEmpty(topic, "topic is required for this request.");
            this.Topic = topic;
        }

        /// <summary>
        /// Updates the room description
        /// </summary>
        /// <param name="desc"></param>
        public void UpdateDescription(string desc)
        {
            Contracts.EnsureNotNullOrEmpty(desc, "description is required for this request.");
            this.Description = desc;
        }

        /// <summary>
        /// Updates the user limit of room
        /// </summary>
        /// <param name="limit"></param>
        public void UpdateLimit(int? limit)
        {
            this.RoomConfiguration.SetLimit(limit);
        }

        /// <summary>
        /// Updates the read receipt of the room
        /// </summary>
        /// <param name="val"></param>
        public void UpdateReadReceipt(bool val)
        {
            this.RoomConfiguration.SetReadReceipt(val);
        }

        /// <summary>
        /// Updates room reaction
        /// </summary>
        /// <param name="val"></param>
        public void UpdateReactions(bool val)
        {
            this.RoomConfiguration.SetReactions(val);
        }

        /// <summary>
        /// Adds a new user to the room
        /// </summary>
        /// <param name="user"></param>
        public void JoinRoom(MessageUser user)
        {
            Contracts.RequiresNotNull(user, "message user is required for this operation.");
            var exist = MessageUserExist(user.id);
            if (exist)
                throw new Exception("cannot join the same room twice.");
            Users.Add(user);
        }

        /// <summary>
        /// Removes a user from the room
        /// </summary>
        /// <param name="userId"></param>
        public void LeaveRoom(string userId)
        {
            Contracts.EnsureNotNullOrEmpty(userId, "user id is required for this request.");
            Users = Users.Where(x => !x.id.EqualsIgnoreCase(userId)).ToList();
        }

        /// <summary>
        /// Adds a new message to the room
        /// </summary>
        /// <param name="message"></param>
        public void AddMessage(MessageBody message)
        {
            Contracts.RequiresNotNull(message, "message is required for this operation.");

            var fromuserExixst = MessageUserExist(message.From.id);

            if(message.To != null)
            {
                var toUserExist = MessageUserExist(message.From.id);
                if (!toUserExist)
                    throw new Exception("the user you are sending this message to doesn't exist");
            }

            if(!fromuserExixst)
                throw new Exception("you can't send a message to a group you haven't joined.");

            Messages.Add(message);
        }

        /// <summary>
        /// Edits a message in the room
        /// </summary>
        /// <param name="request"></param>
        public void EditMessage(EditMessageRequest request)
        {
            Contracts.RequiresNotNull(request, "edit message request is required for this operation.");
            var message = GetMessage(request.MessageId, request.UserId);
            message.EditMessage(request.Message);

        }

        /// <summary>
        /// Deletes a message from the room
        /// </summary>
        /// <param name="request"></param>
        public void DeleteMessage(DeleteMessageRequest request)
        {
            Contracts.RequiresNotNull(request, "delete message request is required for this operation.");
            var message = GetMessage(request.MessageId, request.UserId);
            message.DeleteMessage();
        }

        /// <summary>
        /// Checks if a user already exist in the room
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool MessageUserExist(string userId) 
            => Users.FirstOrDefault(x => x.id.EqualsIgnoreCase(userId)) != null;
    }

    /// <summary>
    /// Room configuration
    /// </summary>
    public record RoomConfiguration
    {
        public int? Limit { get; private set; } = 2;
        public bool IsUnlimited => Limit.HasValue ? false : true;
        public bool ReadReceipt { get; private set; }
        public bool Reactions { get; private set; }

        public RoomConfiguration() { }

        /// <summary>
        /// Created a new room configuration
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="enableReadReceipt"></param>
        /// <param name="enableReactions"></param>
        public RoomConfiguration(int? limit = null, bool enableReadReceipt = false,
            bool enableReactions = false)
        {
            SetLimit(limit);
            this.ReadReceipt = enableReadReceipt;
            this.Reactions = enableReactions;
        }

        /// <summary>
        /// Sets the limit for the amount of participants can be in the room
        /// </summary>
        /// <param name="limit"></param>
        public void SetLimit(int? limit)
        {
            if(limit.HasValue && limit.Value < 2)
            {
                limit = 2;
            }

            this.Limit = limit;
        }

        /// <summary>
        /// Updates whether read recipt is on or off for the room
        /// </summary>
        /// <param name="enableReadReceipt"></param>
        public void SetReadReceipt(bool enableReadReceipt)
        {
            this.ReadReceipt = enableReadReceipt;
        }

        /// <summary>
        /// Update whether reactions can or can't be used in the room
        /// </summary>
        /// <param name="enableReactions"></param>
        public void SetReactions(bool enableReactions)
        {
            this.Reactions = enableReactions;
        }
    }
}
