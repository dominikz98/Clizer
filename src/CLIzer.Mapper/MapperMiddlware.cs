using CLIzer.Contracts;
using CLIzer.Mapper.Contracts;
using CLIzer.Mapper.Models;

namespace CLIzer.Mapper
{
    internal class MapperMiddlware : IClizerMiddleware
    {
        private readonly IClizerMapper _mapper;
        private readonly IClizerFileAccessor<ClizerDictionary> _file;

        public MapperMiddlware(IClizerMapper mapper, IClizerFileAccessor<ClizerDictionary> file)
        {
            _mapper = mapper;
            _file = file;
        }

        public async Task<ClizerPostAction> Intercept(string[] args, CancellationToken cancellationToken)
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
