using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E2ECHATAPI.Services.MessageServices
{
    public class RoomService
    {
        readonly IRoomRepository db;

        private RoomService(IRoomRepository db)
        {
            this.db = db;
        }

        /// <summary>
        /// Creates a single instance of the room service
        /// </summary>
        public static readonly Lazy<Task<RoomService>> Instance = new(async () =>
        {
            var db = await RoomRepository.Instance.Value;
            return new(db);
        }, true);


    }
}
