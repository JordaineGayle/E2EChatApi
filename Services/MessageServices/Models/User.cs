using E2ECHATAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E2ECHATAPI.Services.MessageServices
{
    /// <summary>
    /// User data transfer object
    /// </summary>
    public record UserDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avater { get; set; }
    }

    /// <summary>
    /// Domain user entity
    /// </summary>
    public record User
    {
        public string id { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Avater { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public string FullName => $"{FirstName ?? "-"} {LastName ?? "-"}";

        /// <summary>
        /// Default constructor
        /// </summary>
        public User() { }

        /// <summary>
        /// Primary constructor, used to create a new instance of the user object.
        /// </summary>
        /// <param name="dto"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public User(UserDTO dto)
        {
            Contracts.RequiresNotNull(dto, "user transfer object is required");
            Contracts.EnsureNotNullOrEmpty(dto.FirstName);
            Contracts.EnsureNotNullOrEmpty(dto.LastName);
            this.FirstName = dto.FirstName;
            this.LastName = dto.LastName;
            this.Avater = dto.Avater;
            this.id = Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// Updates basic user information such as name and avatar
        /// </summary>
        /// <param name="dto"></param>
        /// <exception cref="ArgumentException"></exception>
        public void UpdateUser(UserDTO dto)
        {
            Contracts.RequiresNotNull(dto, "user transfer object is required");
            
            if (!dto.FirstName.IsNullOrEmptyWhitespace())
            {
                this.FirstName = dto.FirstName;
            }

            if (!dto.LastName.IsNullOrEmptyWhitespace())
            {
                this.LastName = dto.LastName;
            }

            if (!dto.Avater.IsNullOrEmptyWhitespace())
            {
                this.Avater = dto.Avater;
            }
        }

        /// <summary>
        /// Creates a message user from the domain entity
        /// </summary>
        /// <param name="owner"></param>
        /// <returns>A compressed/minifed user for message visibility</returns>

        public MessageUser CreateMessageUser(bool owner = false)
        {
            return new(id,FullName,Avater,owner);
        }
    }

    /// <summary>
    /// Used to facility user visibility when sending messages
    /// </summary>
    public record MessageUser(string id, string Name, string Avatar, bool Owner = false);
}
