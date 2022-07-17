using E2ECHATAPI.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E2ECHATAPI.Services.UserServices
{
    public interface IUserRespository
    {
        public User Get(string id);
        public User GetByEmail(string enail);
        public Task<User> UpdateAsync(User item);
        public Task<User> DeleteAsync(string id);
    }

    public class UserRespository : IUserRespository
    {
        readonly ConcurrentDictionary<string, User> users;
        readonly IPersistenceLayer db;

        private UserRespository(IPersistenceLayer db,
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
        public static readonly Lazy<Task<UserRespository>> Instance = new(async () =>
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

        public async Task<User> UpdateAsync(User item)
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
