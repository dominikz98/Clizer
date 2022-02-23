# Utils
Here a collection of some usefull classes and functionalities:

## Console Extensions
Use the ConsoleExtensions class to print colored code or print text on specified positions in the console:

```csharp
ConsoleExtensions.AppendToLine(ConsoleColor.White, 5, "10%");
ConsoleExtensions.AppendToLine(ConsoleColor.White, 5, "50%");
ConsoleExtensions.AppendToLine(ConsoleColor.White, 5, "100%");
```

```csharp
ConsoleExtensions.WriteColoredLine(ConsoleColor.Red, "ERROR");
```


## File Accessor
With IClizerFileAccessor you can create and inject an file accessor to encapsulate file managment.
File accessor can be used to load data from io, databases, file storages, ...

Json file from IO accessor example:
```csharp
public class JsonFileByRelativePathAccessor<T> : IClizerFileAccessor<T> where T : class, new()
{
    public string RelativePath { get; }

    public string? Path => System.IO.Path.Combine(Environment.CurrentDirectory, RelativePath);

    public JsonFileByRelativePathAccessor(string relativePath)
    {
        RelativePath = relativePath;
    }

    public async Task<T?> Load(CancellationToken cancellationToken)
    {
        if (Path is null)
            return null;

        using var fs = new FileStream(Path, FileMode.OpenOrCreate);

        if (fs.Length == 0)
            return null;

        var parsed = await JsonSerializer.DeserializeAsync<T>(fs, cancellationToken: cancellationToken);
        if (parsed is null)
            return null;

        return parsed;
    }

    public async Task Save(T data, CancellationToken cancellationToken)
    {
        if (Path is null)
            return;

        using var fs = new FileStream(Path, FileMode.Create);
        await JsonSerializer.SerializeAsync(fs, data, cancellationToken: cancellationToken);
    }
}
```