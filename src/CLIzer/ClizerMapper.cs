using CLIzer.Contracts;
using CLIzer.Models.Mapper;

namespace CLIzer;

public class ClizerMapper : IClizerMapper
{
    internal ClizerDictionary Storage { get; set; }

    private readonly IClizerDataAccessor<ClizerDictionary> _file;
    private readonly Random _rnd;

    public ClizerMapper(IClizerDataAccessor<ClizerDictionary> file)
    {
        _rnd = new Random();
        Storage = new ClizerDictionary();
        _file = file;
    }

    public async Task<ClizerMapping<T>> MapId<T>(T entity, Func<T, Guid> getid, int lifetime, CancellationToken cancellationToken) where T : class
        => (await MapIds(new List<T>() { entity }, getid, lifetime, cancellationToken)).ToArray()[0];

    public async Task<IReadOnlyCollection<ClizerMapping<T>>> MapIds<T>(List<T> entities, Func<T, Guid> getid, int lifetime, CancellationToken cancellationToken) where T : class
    {
        var result = new List<ClizerMapping<T>>();

        Storage.RemoveExpiredMappings();
        var existingShortIds = Storage.GetShortIds<T>();

        foreach (var entity in entities)
        {
            int shortId;
            do
            {
                shortId = _rnd.Next(1000, 10000);
            } while (existingShortIds.Contains(shortId));

            shortId = Storage.Set<T>(getid(entity), shortId, lifetime);
            result.Add(new ClizerMapping<T>(shortId, entity));
        }

        await _file.Save(Storage, cancellationToken);
        return result;
    }

    public Guid? GetByShortId<T>(int shortid) where T : class
        => Storage.GetByShortId<T>(shortid);
}
