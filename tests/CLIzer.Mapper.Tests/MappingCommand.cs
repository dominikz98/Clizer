using CLIzer.Contracts;
using CLIzer.Mapper.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace CLIzer.Mapper.Tests
{
    internal class MappingCommand : ICliCmd
    {
        private readonly IClizerMapper _mapper;

        public MappingCommand(IClizerMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<int> Execute(CancellationToken cancellationToken)
        {
            var entity1 = new TestEntity();
            var mapping1 = await _mapper.MapId(entity1, x => x.Id, 10, default);
            var reverse1 = _mapper.GetByShortId<TestEntity>(mapping1.ShortId);

            return (int)ClizerExitCodes.SUCCESS;
        }
    }
}
