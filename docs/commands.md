# Commands

Commands can be registered with in the Configure() method.
All commands will be automatically injected, so dont worry about that.
You can use the following 4 methods to dot it at a hierarchically or flat way:

## Root()
You want to execute a command if the assembly is directly called?
So there you go.

Commands passed in this method, will be registered directly in the 'root path' of the command container.
Here a little example in reference to the structure in the below area:
```batch
Call:
C:\Users\Spongebob> {{assembly}}

Output:
Hello from Root Command :0
```

## Command()
This method registers commands in a 'flat' level and returns an instance of its own registration.
Concated Command() calls will all be at the same call level:

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
Here commands can be registered with an dependency to its predecessor.
The method returns an instance of the new registered one.
Here a little example in reference to the structure in the below area:
```batch
C:\Users\Spongebob> {{assembly}} company employee add
```

## Return()
So you already registered many sub commands, but dont know how to get back to root level?
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