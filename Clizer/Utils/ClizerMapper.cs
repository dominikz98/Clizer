using Clizer.Constants;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Clizer.Utils
{
    public static class ClizerMapper
    {
        private static readonly ClizerDictionary _storage;
        private static readonly Random _rnd;

        static ClizerMapper()
        {
            _rnd = new Random();
            try
            {
                var content = File.ReadAllText(ClizerConstants.MappingFile);
                _storage = JsonConvert.DeserializeObject<ClizerDictionary>(content);
            }
            catch (Exception)
            {
                _storage = new ClizerDictionary();
            }
        }

        public static ClizerMapping<T> MapId<T>(T entity, Func<T, Guid> getid, int lifetime) where T : class
            => MapIds<T>(new List<T>() { entity }, getid, lifetime)[0];

        public static List<ClizerMapping<T>> MapIds<T>(List<T> entities, Func<T, Guid> getid, int lifetime) where T : class
        {
            var result = new List<ClizerMapping<T>>();

            _storage.RemoveExpiredMappings();
            var existingShortIds = _storage.GetShortIds<T>();

            foreach (var entity in entities)
            {
                int shortId;
                do
                {
                    shortId = _rnd.Next(1000, 10000);
                } while (existingShortIds.Contains(shortId));

                shortId = _storage.Set<T>(getid(entity), shortId, lifetime);
                result.Add(new ClizerMapping<T>(shortId, entity));
            }

            File.WriteAllText(ClizerConstants.MappingFile, JsonConvert.SerializeObject(_storage));
            return result;
        }

        public static Guid? GetByShortId<T>(int shortid) where T : class
            => _storage.GetByShortId<T>(shortid);
    }

    internal class ClizerDictionary
    {
        public Dictionary<Type, List<(Guid Id, int ShortId, DateTime ExpiresAt)>> Mappings { get; set; }

        public ClizerDictionary()
        {
            Mappings = new Dictionary<Type, List<(Guid Id, int ShortId, DateTime ExpiresAt)>>();
        }

        public int? GetById<T>(Guid id)
        {
            if (!Mappings.ContainsKey(typeof(T)))
                return null;

            var entry = Mappings[typeof(T)].FirstOrDefault(x => x.Id == id);
            if (entry == default)
                return null;

            return entry.ShortId;
        }

        public Guid? GetByShortId<T>(int shortid)
        {
            if (!Mappings.ContainsKey(typeof(T)))
                return null;

            var entry = Mappings[typeof(T)].FirstOrDefault(x => x.ShortId == shortid);
            if (entry == default)
                return null;

            return entry.Id;
        }

        public List<int> GetShortIds<T>() where T : class
        {
            if (!Mappings.ContainsKey(typeof(T)))
                return new List<int>();

            return Mappings[typeof(T)].Select(x => x.ShortId)?.ToList() ?? new List<int>();
        }

        public int Set<T>(Guid id, int shortid, int lifetime)
        {
            if (!Mappings.ContainsKey(typeof(T)))
                Mappings[typeof(T)] = new List<(Guid Id, int ShortId, DateTime ExpiresAt)>();

            var entry = Mappings[typeof(T)].FirstOrDefault(x => x.Id == id);
            var expiresAt = DateTime.Now.AddMinutes(lifetime);

            if (entry != default)
            {
                entry.ExpiresAt = expiresAt;
                return entry.ShortId;
            }

            Mappings[typeof(T)].Add((id, shortid, expiresAt));

            return shortid;
        }

        public void RemoveExpiredMappings()
        {
            foreach (var entry in Mappings.ToList())
            {
                foreach (var mapping in entry.Value.Where(x => x.ExpiresAt <= DateTime.Now)?.ToList() ?? new List<(Guid Id, int ShortId, DateTime ExpiresAt)>())
                    entry.Value.Remove(mapping);
                if (entry.Value.Count == 0)
                    Mappings.Remove(entry.Key);
            }
        }
    }

    public class ClizerMapping<T> where T : class
    {
        public int ShortId { get; set; }
        public T Entity { get; set; }

        public ClizerMapping(int shortid, T entity)
        {
            ShortId = shortid;
            Entity = entity;
        }
    }
}
