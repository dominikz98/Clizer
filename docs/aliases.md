# Aliases
With the EnableAliases() method in the configuration section, its possible to enable user configurable aliases.
Various ways can be passed into the method to tell the framework on which way the user defines them.
Aliases could used nested, and regardless of the place of the call (See example).

## Format
```
{name} = {command 1} {command 2} ...
```
If there is an incorrectly configured entry in a line, it will simply be omitted.

## Example
Call
```batch
C:\Users\Spongebob> {{assembly}} new
```

Aliases file
```txt
new = cook krabby_patty xsp
xsp = extra --spicy
```

Resolved call
```batch
C:\Users\Spongebob> {{assembly}} cook krabby_patty extra --spicy
```