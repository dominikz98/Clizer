using CLIzer.Models.Mapper;

namespace CLIzer.Contracts
{
    public interface IClizerMapper
    {
        Task<ClizerMapping<T>> MapId<T>(T entity, Func<T, Guid> getid, int lifetime, CancellationToken cancellationToken) where T : class;
        Task<IReadOnlyCollection<ClizerMapping<T>>> MapIds<T>(List<T> entities, Func<T, Guid> getid, int lifetime, CancellationToken cancellationToken) where T : class;
        Guid? GetByShortId<T>(int shortid) where T : class;
    }
}
