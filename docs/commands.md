# Commands
Commands can be registered with in the Configure() method.
All commands will be automatically injected, so don't worry about that.
You can use the following 4 methods to dot it in a hierarchically or flat way:

## Root()
You want to execute a command if the assembly is directly called?
So there you go.

Commands passed in this method, will be registered directly in the 'root path' of the command container.
Here is a little example in reference to the structure in the below area:
```batch
Call:
C:\Users\Spongebob> {{assembly}}

Output:
Hello from Root Command :0
```

## Command()
This method registers commands in a 'flat' level and returns an instance of its own registration.
Concatenated Command() calls will all be at the same call level:

```csharp
.Configure((config) => config
    .RegisterCommands((container) => container
        .Command<ThaddeusCmd>("thaddeus")
        .Command<PatrickCmd>("patrick")
    )
)
```

```batch
C:\Users\Spongebob> {{assembly}} thaddeus
C:\Users\Spongebob> {{assembly}} patrick
```

## SubCommand()
Here commands can be registered with a dependency to its predecessor.
The method returns an instance of the new registered one.
Here is a little example in reference to the structure in the below area:
```batch
C:\Users\Spongebob> {{assembly}} company employee add
```

## Return()
So you already registered many sub commands, but don't know how to get back to root level?
Just call Return() and get the instance of the command container.

# Example
```csharp
var clizer = new Clizer()
    .Configure((config) => config

    .RegisterCommands((container) => container
        .Root<RootCmd>()
        .Command<CompanyCmd>("company")
            .SubCommand<EmployeeCmd>("employee")
                .SubCommand<AddEmployeeCmd>("add")
        .Return()
        .Command<GrandparentCmd>("grandparent")
            .SubCommand<ParentCmd>("parent")
                .SubCommand<ChildCmd>("child")
    )
);
```