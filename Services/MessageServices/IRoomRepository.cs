﻿using E2ECHATAPI.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E2ECHATAPI.Services.MessageServices
{
    public interface IRoomRepository
    {
        public Room Get(string id);
        public IEnumerable<Room> GetRoomsByUserId(string id);
        public Task<Room> UpdateAsync(Room item);
        public Task<Room> DeleteAsync(string id);
    }

    public class RoomRepository : IRoomRepository
    {
        readonly ConcurrentDictionary<string, Room> rooms;
        readonly IPersistenceLayer db;

        private RoomRepository(IPersistenceLayer db,
            ConcurrentDictionary<string, Room> rooms)
        {
            this.db = db;
            this.rooms = rooms;
            if (this.rooms == null)
                this.rooms = new();
        }

        /// <summary>
        /// Creates a single instance of the room respository
        /// </summary>
        public static readonly Lazy<Task<RoomRepository>> Instance = new(async () =>
        {
            var db = new PersistenceLayer("rooms.json");
            var rooms = await db.RetrieveAsync<ConcurrentDictionary<string, Room>>();
            return new(db, rooms);

        }, true);


        public Room Get(string id)
        {
            Contracts.EnsureNotNullOrEmpty(id, "rooms's id is mandatory.");
            rooms.TryGetValue(id, out Room room);
            return room;
        }


        public IEnumerable<Room> GetRoomsByUserId(string id)
            => rooms.Values.Where(x => x.Users.ToList().Exists(u => u.id.EqualsIgnoreCase(id)));

        public async Task<Room> UpdateAsync(Room item)
        {
            Contracts.RequiresNotNull(item, "room to be updated is required.");
            item = rooms.AddOrUpdate(item.id, item, (i, n) => item);
            await db.SaveAsync(item);
            return item;
        }

        public async Task<Room> DeleteAsync(string id)
        {
            Contracts.EnsureNotNullOrEmpty(id, "rooms's id is mandatory.");
            rooms.TryRemove(id, out Room room);
            await db.SaveAsync(rooms);
            return room;
        }

    }
}