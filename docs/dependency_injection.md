# Dependency Injection
With the RegisterServices() method in the configuration section a delegate to configure a service collection or an instance of itself can be passed.

# Example
```csharp
new Clizer()
    .Configure((config) => config
        .RegisterServices((services) => services

            .AddSingleton<IBurgerFactory, BurgerFactory>()
            .AddSingleton<Appetizer>())
    );
```