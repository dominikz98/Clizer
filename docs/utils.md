# Utils
Here is a collection of some useful classes and functionalities:

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

## Cancellation Binding
Passed CancellationTokens will automatecally be triggerd by pressing STRG + C.

## Data Accessor
With IClizerFileAccessor you can create and inject a data accessor to encapsulate file management.
Data accessor can be used to load data from IO, databases, file storage, ...

Json file from IO accessor example:
```csharp
public class JsonFileAccessor<T> : IClizerFileAccessor<T> where T : class, new()
{
    public string RelativePath { get; }

    public string? Path => System.IO.Path.Combine(Environment.CurrentDirectory, RelativePath);

    public JsonFileAccessor(string relativePath)
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