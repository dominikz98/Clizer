namespace CLIzer.Contracts
{
    internal class PersistNameResolver<TCmd> : ICommandNameResolver where TCmd : ICliCmd
    {
        public string Name { get; }
        public ICommandNameResolver Fallback { get; }

        public PersistNameResolver(string name, ICommandNameResolver fallback)
        {
            Name = name;
            Fallback = fallback;
        }

        public string Resolve<T>() where T : ICliCmd
            => Resolve(typeof(T));

        public string Resolve(Type cmdType)
        {
            if (cmdType != typeof(TCmd))
                return Fallback.Resolve(cmdType);

            return Name;
        }
    }
}
