# C# Best Practices

## Naming Conventions

Naming things, and furthermore well naming them is very important.
You have to use clear and descriptive names for classes, methods, fields and variables for easy understanding and maintenance.

1. Generally make sure that after refactoring code that the names are still appropriate.

For example here is a method that does not reflect its real behavior, which is bad.

```csharp
// The method name includes the text "Not" and returns a boolean value.
// This could create scenarios where callers are performing double
// negatives, which are hard to read.
private static bool IsNotConnected(SocketWrapper socket, string serverName)
{
    // The behavior of the method currently checks if socket is connected to the provided
    // server. If not it tries to connect to it. But this is not reflected in the method name
    // at all and is only discoverable by a developer inspecting the code.
    if (socket.Server != serverName)
    {
        socket.Disconnect();
    }

    if (socket.IsConnected())
        return false;

    return !socket.Connect(serverName);
}
```

Here is an improved name and signature:

```csharp
private static bool TryConnect(SocketWrapper socket, string serverName)
{
    // ...
}
```

2. Use the following guidelines when defining generic types:

    a. For types with a single generic type, prefer the use of `T`, except if a more precise name is adapted (e.g. `TEdge`, etc.).

    ```csharp
    public interface ISomething<T>
    ```

    b. For types with multiple generic types, use a capital `T` followed by a secondary name for clarity. In all cases, start the type with a capital letter.

    ```csharp
    public interface IService<TRequest, TResponse>
    ```

3. Name custom attribute and exception classes respectively using suffixes `Attribute` and `Exception`.

4. Name enumerations in the singular (e.g., `FileMode`) unless the enumeration is representing a bit flag value with `[Flags]` attribute (e.g., `FileAttributes`).

## Exception Handling

1. Express exception messages as grammatically correct sentences.

2. Before throwing Exception, consider whether a custom exception or one of the following .NET exceptions would provide semantic value:

    a. [ArgumentException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentexception)

    b. [ArgumentOutOfRangeException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentoutofrangeexception)

    c. [ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)

    d. [IndexOutOfRangeException](https://docs.microsoft.com/en-us/dotnet/api/system.indexoutofrangeexception)

    e. [InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/system.invalidoperationexception) (According to Microsoft .NET documentation, "Typically, it is thrown when the state of an object cannot support the method call" such as when an instance variable is null and therefore you can't reference a property on that variable)

    f. [NotSupportedException](https://docs.microsoft.com/en-us/dotnet/api/system.notsupportedexception) / [NotImplementedException](https://docs.microsoft.com/en-us/dotnet/api/system.notimplementedexception)

3. Do not reuse .NET exceptions outside of their semantic context.

## Using Static Structures

1. Call static members by using the class name: `ClassName.StaticMember`.
This practice makes code more readable by making static access clear.

2. Do not qualify a static member defined in a base class with the name of a derived class. While that code compiles, the code readability is misleading, and the code may break in the future if you add a static member with the same name to the derived class.

3. Avoid taking dependencies on static .NET Framework classes whose results are not idempotent or require access to infrastructure (e.g., `System.DateTime`, `System.IO.File`, `System.IO.Path`, etc.).
Instead, define an interface using a naming convention of `I`{_StaticClassName_} and build a minimal facade with an implementation named as {_StaticClassName_}`Wrapper`.
This will enable the behavior to be controlled for testing purposes.

As an example for `System.IO.File`, this would look like: 

```csharp
public interface IFile
{
    bool Exists(string path);
}

public class FileWrapper : IFile
{
    public bool Exists(string path)
    {
        return File.Exists(path);
    }
}
```

## Design Guidance
### SOLID Principles

Apply SOLID principles everywhere!

1. _Single Responsibility Principle_: There should only be one reason for a class to change. If you're having to change an existing class, ask yourself, "Is there an abstraction (interface) missing here?"
2. _Open/Closed Principle_: The system should be open for extension, but closed to modification. You should be able to change the behavior of the system by adding a new implementation of an existing interface.
3. _Liskov Substitution Principle_: A derived class should be substitutable for its base class, and should not change the fundamental nature of the abstraction.
4. _Interface Segregation Principle_: An interface should be highly focused. Interfaces may just have one or two methods. If you implement some methods of an interface using `new NotImplementedException()`, the interface probably needs to be decomposed.
5. _Dependency Inversion Principle_: External dependencies are injected into classes, preferably via their constructors.

### Class Design

1. Avoid deep class hierarchies (more than 2 total levels). Prefer composition over inheritance.

2. Keep logic in constructors simple, primarily focused on capturing dependencies to the field values.

3. Do not use properties to modify class state outside of the property being set. If other state must be changed, prefer the use of a read-only property in combination with a well-named method.

4. Prefer the use of return types that are abstractions rather than concrete types from public members. In other words, prefer `IList<T>` to `List<T>` and `IDictionary<TKey, TValue>` over `Dictionary<TKey, TValue>`.

5. Make sure the return types from public members match the intended semantics. In other words, prefer `IEnumerable<T>` over `IList<T>` and `IReadOnlyDictionary<TKey, TValue>` over `IDictionary<TKey, TValue>`, unless modifications by the caller are intended.

```csharp
// Ok
public interface IStudentDataProvider
{
    IEnumerable<Student> GetAll();
    IReadOnlyDictionary<string, Student> GetStudentByNameDictionary();
}

// Potentially problematic for maintenance, due to semantics of returned values
public interface IStudentDataProvider
{
    IList<Student> GetAll();
    IDictionary<string, Student> GetStudentByNameDictionary();
}
```

6. Do not use the `new` inheritance qualifier. Instead, reevaluate the design of the class.

7. Do not make members of a class `public` for the sake of unit testing. Find a better way to test the functionality.
Otherwise consider making things `internal` if there is really no other solutions.

8. When providing an implementation of an interface that does nothing, use the [Null Object Pattern](https://en.wikipedia.org/wiki/Null_object_pattern).

```csharp
public interface ISomething
{
    DateTime GetDateTime(string parameter);
}

public class NullSomething : ISomething
{
    public DateTime GetDateTime(string parameter)
    {
        return default(DateTime);
    }
}
```

9. The Provider Pattern (also known as [Strategy Pattern](https://en.wikipedia.org/wiki/Strategy_pattern)) is used heavily in the code base.

    a. If the contract being defined basically allows the caller to _get_ something, the `Provider` suffix is preferred.

    b. If the contract allows the caller to _get_ and _set_ something, consider splitting the methods into `Reader` and `Writer` interfaces (which could still be implemented by the same class). The intent here is to provide more clearly stated intent when a caller intends to modify the underlying data exposed by the provider.

    ```csharp
    // Instead of doing this...
    public interface ICacheProvider
    {
        void RemoveObjects(string keys);
        void RemoveObject(string key);
        bool TryGetObject(string key, out object value);
        void SetObject(string key, object obj);
        void InsertObject(string key, object value, DateTime absoluteExpiration, TimeSpan slidingExpiration);
    }

    // Consider decomposing the interfaces like this
    public interface ICacheReader
    {
        bool TryGetObject(string key, out object value);
    }

    public interface ICacheWriter
    {
        void RemoveObjects(string keys);
        void RemoveObject(string key);
        void SetObject(string key, object obj);
        void InsertObject(string key, object value, DateTime absoluteExpiration, TimeSpan slidingExpiration);
    }
    ```

10. Use creational patterns for creating objects that involve significant logic.

    a. When the logic required to create something doesn't fit well into a class constructor, use a creational pattern like the [Factory Pattern](https://en.wikipedia.org/wiki/Factory_%28object-oriented_programming%29).

    b. When an item can built in one step, use "Create" semantics (see the [Factory Method Pattern](https://en.wikipedia.org/wiki/Factory_method_pattern)).

    c. When an item is built up over multiple steps, using "Build" semantics (see the [Builder Pattern](https://en.wikipedia.org/wiki/Builder_pattern)).

11. When a class has the concept of an empty instance, implement a static read-only `Empty` property.

```csharp
public class SomeObject
{
    public static readonly SomeObject Empty = new SomeObject();
    // ...
}
```