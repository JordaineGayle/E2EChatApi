using E2ECHATAPI.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E2ECHATAPI.Services.UserServices
{
    public interface IUserRepository
    {
        /// <summary>
        /// Gets a user by id from the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public User Get(string id);

        /// <summary>
        /// Gets a user by email from the database
        /// </summary>
        /// <param name="enail"></param>
        /// <returns></returns>
        public User GetByEmail(string enail);

        /// <summary>
        /// Add or updates a user in the database
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Task<User> UpsertAsync(User item);

        /// <summary>
        /// Deletes a user from the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<User> DeleteAsync(string id);
    }

    public class UserRepository : IUserRepository
    {
        readonly ConcurrentDictionary<string, User> users;
        readonly IPersistenceLayer db;

        private UserRepository(IPersistenceLayer db,
            ConcurrentDictionary<string, User> users)
        {
            this.db = db;
            this.users = users;
            if(this.users == null)
                this.users = new();
        }

        /// <summary>
        /// Creates a single instance of the user respository
        /// </summary>
        public static readonly Lazy<Task<UserRepository>> Instance = new(async () =>
        {
            var db = new PersistenceLayer("users.json");
            var users = await db.RetrieveAsync<ConcurrentDictionary<string, User>>();
            return new(db, users);

        }, true);

        
        public User Get(string id)
        {
            Contracts.EnsureNotNullOrEmpty(id, "user's id is mandatory.");
            users.TryGetValue(id, out User user);
            return user;
        }

        public User GetByEmail(string email)
        {
            Contracts.EnsureNotNullOrEmpty(email, "user's email is mandatory.");
            var user = users.Values.FirstOrDefault(x => x.Email.EqualsIgnoreCase(email));
            return user;
        }

        public async Task<User> UpsertAsync(User item)
        {
            Contracts.RequiresNotNull(item, "user to be updated is required.");
            item = users.AddOrUpdate(item.id, item, (i, n) => item);
            await db.SaveAsync(users);
            return item;
        }

        public async Task<User> DeleteAsync(string id)
        {
            Contracts.EnsureNotNullOrEmpty(id, "user's id is mandatory.");
            users.TryRemove(id, out User user);
            await db.SaveAsync(users);
            return user;
        }
    }
}
