using CLIzer.Contracts;

namespace CLIzer.Models
{
    internal class ConfigProvider<T> : IConfig<T> where T : class, new()
    {
        public T? Value { get; set; }
    }
}
