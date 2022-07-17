using E2ECHATAPI.Helpers;
using System;
using System.ComponentModel.DataAnnotations;

namespace E2ECHATAPI.Services.UserServices
{
    
    /// <summary>
    /// Domain user entity
    /// </summary>
    public record User
    {
        public string id { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Avater { get; init; }
        public string Email { get; init; }
        public string Password { get; init; }
        public string FullName => $"{FirstName ?? "-"} {LastName ?? "-"}";

        /// <summary>
        /// Default constructor
        /// </summary>
        public User() { }

        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static User RegisterUser(RegisterUserRequest dto)
        {
            Contracts.RequiresNotNull(dto, "user transfer object is required");
            Contracts.EnsureNotNullOrEmpty(dto.FirstName);
            Contracts.EnsureNotNullOrEmpty(dto.LastName);
            Contracts.EnsureNotNullOrEmpty(dto.Email);
            Contracts.EnsureNotNullOrEmpty(dto.Password);
            var user = new User 
            {
                id = Guid.NewGuid().ToString("N"),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Avater = dto.Avatar,
                Email = dto.Email,
                Password = dto.Password
            };
            return user;
        }

        /// <summary>
        /// Handles user login on the current user instance
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public MinifiedUser Login(LoginRequest request)
        {
            Contracts.RequiresNotNull(request, "login request is required for this request.");
            Contracts.EnsureNotNullOrEmpty(request.Email, "email address id required for this request.");
            Contracts.EnsureNotNullOrEmpty(request.Password, "password is required for this request.");
            if (request.Email.EqualsIgnoreCase(Email) && request.Password == Password)
                return CreateMinifiedUser();
            throw new UnauthorizedAccessException("invalid login credentials.");
        }


        /// <summary>
        /// Updates the user's firstname and returns a new instance of the user entity
        /// </summary>
        /// <param name="fname"></param>
        /// <returns></returns>
        public User UpdateFirstName(string fname)
        {
            Contracts.EnsureNotNullOrEmpty(fname, "firstname is required for this request.");
            return this with { FirstName = fname };
        }

        /// <summary>
        /// Updates the user's lastname and returns a new instance of the user entity
        /// </summary>
        /// <param name="lname"></param>
        public User UpdateLastName(string lname)
        {
            Contracts.EnsureNotNullOrEmpty(lname, "lastname is required for this request.");
            return this with { LastName = lname };
        }

        /// <summary>
        /// Updates the user's avatar and returns a new instance of the user entity
        /// </summary>
        /// <param name="avatar"></param>
        public User UpdateAvatar(string avatar)
        {
            Contracts.EnsureNotNullOrEmpty(avatar, "avatar is required for this request.");
            return this with { Avater = avatar };
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

        /// <summary>
        /// Gets a minified version for the user to return to the client side
        /// </summary>
        /// <returns></returns>
        public MinifiedUser CreateMinifiedUser()
        {
            return new(id, FirstName, LastName, Avater);
        }
    }

    /// <summary>
    /// User login request entity
    /// </summary>
    public record LoginRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

    /// <summary>
    /// Registration request entity
    /// </summary>
    public record RegisterUserRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required] 
        public string FirstName { get; set; }

        [Required] 
        public string LastName { get; set; }
        
        public string Avatar { get; set; }
    }

    /// <summary>
    /// Message user entity
    /// </summary>
    public record MessageUser(string id, string Name, string Avatar, bool Owner = false);

    /// <summary>
    /// Minified user entity
    /// </summary>
    public record MinifiedUser(string Token, string FirstName, string LastName, string Avatar);
}
