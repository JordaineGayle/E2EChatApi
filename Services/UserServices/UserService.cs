﻿using E2ECHATAPI.Helpers;
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

        public async Task<MinifiedUser> RegisterUserAsync(RegisterUserRequest request)
        {
            var user = User.RegisterUser(request);
            user = await db.UpsertAsync(user);
            return user.CreateMinifiedUser();
        }

        public MinifiedUser Login(LoginRequest request)
        {
            var user = db.GetByEmail(request?.Email);
            var res = user?.Login(request);
            if (res == null)
                throw new UnauthorizedAccessException("invalid credentials");
            return res;
        }

        public async Task<MinifiedUser> UpdateFirstNameAsync(RequestContext ctx, string fname)
        {
            var user = GetUser(ctx.User.id);
            user = user.UpdateFirstName(fname);
            user = await db.UpsertAsync(user);
            return user.CreateMinifiedUser();
        }

        public async Task<MinifiedUser> UpdateLastNameAsync(RequestContext ctx, string lname)
        {
            var user = GetUser(ctx.User.id);
            user = user.UpdateLastName(lname);
            user = await db.UpsertAsync(user);
            return user.CreateMinifiedUser();
        }

        public async Task<MinifiedUser> UpdateAvatarAsync(RequestContext ctx, string avatar)
        {
            var user = GetUser(ctx.User.id);
            user = user.UpdateAvatar(avatar);
            user = await db.UpsertAsync(user);
            return user.CreateMinifiedUser();
        }

    }
}
