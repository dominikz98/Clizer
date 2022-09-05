using CLIzer.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CLIzer.Tests.Mapper;

internal class MappingCommand : ICliCmd
{
    private readonly IClizerMapper _mapper;

    public MappingCommand(IClizerMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task<ClizerExitCode> Execute(CancellationToken cancellationToken)
    {
        var entity = new TestEntity();
        var mapping = await _mapper.MapId(entity, x => x.Id, 10, default);
        Console.WriteLine($"[ID MAPPING]: {entity.Id} -> {mapping.ShortId}");

        var reverse = _mapper.GetByShortId<TestEntity>(mapping.ShortId);
        Console.WriteLine($"[ID MAPPING]: {mapping.ShortId} -> {reverse}");

        return ClizerExitCode.SUCCESS;
    }
}
