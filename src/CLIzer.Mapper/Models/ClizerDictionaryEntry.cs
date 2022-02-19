namespace CLIzer.Mapper.Models
{
    public class ClizerDictionaryEntry
    {
        public Guid Id { get; set; }
        public int ShortId { get; set; }
        public DateTime ExpiresAt { get; set; }

        public ClizerDictionaryEntry(Guid id, int shortId, DateTime expiresAt)
        {
            Id = id;
            ShortId = shortId;
            ExpiresAt = expiresAt;
        }

        public ClizerDictionaryEntry() { }
    }
}
