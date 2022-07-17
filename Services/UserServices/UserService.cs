using E2ECHATAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E2ECHATAPI.Services.UserServices
{
    public class UserService
    {
        readonly IUserRespository db;

        private UserService(IUserRespository db)
        {
            this.db = db;
        }

        /// <summary>
        /// Creates a single instance of the user service
        /// </summary>
        public static readonly Lazy<Task<UserService>> Instance = new(async () =>
        {
            var db = await UserRespository.Instance.Value;
            return new(db);
        }, true);

        public User GetUser(string id)
        {
            var user = db.Get(id).EnsureNotNull();
            return user;
        }

        public User TryGetUser(string id)
        {
            var user = db.Get(id);
            if (user == null)
                return null;
            return user;
        }

    }
}
