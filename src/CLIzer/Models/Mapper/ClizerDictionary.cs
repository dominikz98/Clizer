namespace CLIzer.Models.Mapper
{
    public class ClizerDictionary
    {
        public Dictionary<string, List<ClizerDictionaryEntry>> Mappings { get; set; }
        public bool IsInitialized => Mappings.Any();

        public ClizerDictionary()
        {
            Mappings = new Dictionary<string, List<ClizerDictionaryEntry>>();
        }

        public int? GetById<T>(Guid id)
        {
            if (!Mappings.ContainsKey(typeof(T).Name))
                return null;

            var entry = Mappings[typeof(T).Name].FirstOrDefault(x => x.Id == id);
            if (entry == default)
                return null;

            return entry.ShortId;
        }

        public Guid? GetByShortId<T>(int shortid)
        {
            if (!Mappings.ContainsKey(typeof(T).Name))
                return null;

            var entry = Mappings[typeof(T).Name].FirstOrDefault(x => x.ShortId == shortid);
            if (entry == default)
                return null;

            return entry.Id;
        }

        public List<int> GetShortIds<T>() where T : class
        {
            if (!Mappings.ContainsKey(typeof(T).Name))
                return new List<int>();

            return Mappings[typeof(T).Name].Select(x => x.ShortId)?.ToList() ?? new List<int>();
        }

        public int Set<T>(Guid id, int shortid, int lifetime)
        {
            if (!Mappings.ContainsKey(typeof(T).Name))
                Mappings[typeof(T).Name] = new List<ClizerDictionaryEntry>();

            var entry = Mappings[typeof(T).Name].FirstOrDefault(x => x.Id == id);
            var expiresAt = DateTime.Now.AddMinutes(lifetime);

            if (entry != default)
            {
                entry.ExpiresAt = expiresAt;
                return entry.ShortId;
            }

            Mappings[typeof(T).Name].Add(new ClizerDictionaryEntry(id, shortid, expiresAt));

            return shortid;
        }

        public void RemoveExpiredMappings()
        {
            foreach (var entry in Mappings.ToList())
            {
                var mappings = entry.Value.Where(x => x.ExpiresAt <= DateTime.Now).ToList();
                foreach (var mapping in mappings)
                    entry.Value.Remove(mapping);
                if (entry.Value.Count == 0)
                    Mappings.Remove(entry.Key);
            }
        }
    }
}
