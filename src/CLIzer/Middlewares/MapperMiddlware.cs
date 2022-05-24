using CLIzer.Contracts;
using CLIzer.Models;
using CLIzer.Models.Mapper;

namespace CLIzer.Middlewares
{
    internal class MapperMiddlware : IClizerMiddleware
    {
        private readonly IClizerMapper _mapper;
        private readonly IClizerDataAccessor<ClizerDictionary> _file;

        public MapperMiddlware(IClizerMapper mapper, IClizerDataAccessor<ClizerDictionary> file)
        {
            _mapper = mapper;
            _file = file;
        }

        public async Task<ClizerPostAction> Intercept(CommandContext context, CancellationToken cancellationToken)
        {
            if (_mapper is not ClizerMapper mapper)
                return ClizerPostAction.CONTINUE;

            var data = await _file.Load(cancellationToken);
            if (data is null)
                return ClizerPostAction.CONTINUE;

            mapper.Storage = data;
            return ClizerPostAction.CONTINUE;
        }
    }
}
