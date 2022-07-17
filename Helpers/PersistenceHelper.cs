using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace E2ECHATAPI.Helpers
{
    public interface IPersistenceLayer
    {
        public Task SaveAsync<T>(T item);
        public Task<T> RetrieveAsync<T>();
    }

    public class PersistenceLayer : IPersistenceLayer
    {
        readonly string fileName;

        public PersistenceLayer(string fileName)
        {
            this.fileName = fileName;
        }

        public async Task<T> RetrieveAsync<T>()
        {
            CreateIfNotExist();
            var data = await File.ReadAllTextAsync(fileName);
            if (string.IsNullOrWhiteSpace(data))
                return default(T);
            return (T)JsonSerializer.Deserialize(data, typeof(T));
        }

        public async Task SaveAsync<T>(T item)
        {
            CreateIfNotExist();
            var data = JsonSerializer.Serialize(item, typeof(T));
            await File.WriteAllTextAsync(fileName, data);
        }

        private void CreateIfNotExist()
        {
            string temp = AppDomain.CurrentDomain.BaseDirectory;
            string sPath = Path.Combine(temp, fileName);
            bool fileExist = File.Exists(sPath);
            if (!fileExist)
            {
                using (File.Create(sPath)) ;
            }
        }
    }
}
