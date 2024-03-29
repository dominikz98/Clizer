﻿namespace CLIzer.Contracts;

public interface IConfig<T> where T : class, new()
{
    public T? Value { get; }
}
