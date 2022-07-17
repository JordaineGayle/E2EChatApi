using E2ECHATAPI.Services.UserServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace E2ECHATAPI.Helpers
{
    public record RequestContext
    {
        readonly DateTimeOffset timestamp = DateTimeOffset.UtcNow;
        public User User { get; init; }
        public MinifiedUser MinifiedUser => User?.CreateMinifiedUser();
        public CancellationToken Cancelled { get; init; }
        public DateTimeOffset Timestamp => this.timestamp;
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }

    public static class RequestHelpers
    {
        public static RequestContext GetRequestContext(this ControllerBase controller)
        {
            var exist = controller.HttpContext.Request.Headers.TryGetValue("ApiKey", out StringValues item);
            if (!exist)
                throw new UnauthorizedAccessException($"ApiKey is required.");

            var userId = item.ToString();
            var svc = UserService.Instance.Value.Result;
            var user = svc.TryGetUser(userId);
            if (user == null)
                throw new UnauthorizedAccessException($"invalid apikey, unable to find user.");

            return new RequestContext
            {
                User = user,
                Cancelled = controller.HttpContext.RequestAborted
            };
        }
    }
}
