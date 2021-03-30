# C# Coding guidelines

This page will describe some guidelines, note also that you can refer to [MSDN guidelines](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions) for reference.
If the MSDN page describes a not matching point, consider this page as reference for the concerned point.

## Naming Conventions

1. Use Pascal casing for type, method names, and constants:

```csharp
public class SomeClass
{
    private const int DefaultSize = 100;

    public void SomeMethod()
    {
    }
}
```

2. Use camel casing for local variable names and method arguments:

```csharp
public void SomeMethod(int someNumber)
{
    int number;
    // ...
}
```

3. Prefix interfaces with `I`:

```csharp
public interface ISomething
{
}
```

4. Suffix interface implementations classes with the non-prefixed interface name:

```csharp
public class ThisSomething : ISomething
{
}
```

5. Prefix all fields with an underscore `_`. If a file happens to differ in style from these guidelines, the existing style in that file takes precedence:

```csharp
public class SomeClass
{
    private ISomething _thisSomething;
}
```

6. Name methods using a verb or verb-object pair.

7. You may use single-character or mnemonic variable names in the following scenarios:

    a. In for loops where it is a common convention to use variables like `i` , `j` , and `k` .
    
    b. In LINQ expressions where it is a common convention to use variables like `x` or a mnemonic for the item being represented (for example `stt` for an object of type `SomeTestType`).

8. Do not abbreviate terms or use acronyms. Some exceptions are allowed for very well-known terms such as API, Id, msg, max, min, etc.

9. Name abstract classes using a suffix of Base (e.g., `SomeClassBase`).

10. Name extension classes and files by the type being extended, and remove `I` prefix for extensions of interfaces:

```csharp
// SomeClassExtensions.cs
public static class SomeClassExtensions
{
    public static int ToInt(this SomeClass input)
    {
        // ...
    }
}

// Or

public interface ISomething
{
}

public static class SomethingExtensions
{
}
```

## Code Formatting

1. Use [Allman style](https://en.wikipedia.org/wiki/Indentation_style#Allman_style) braces where each brace begins on a new line and use consistent style.
Only exceptions to braces are for single `return` or `throw` statements.

```csharp
// Ok
if (a == b)
{
    c = d;
}

// Acceptable
if (a == b)
    return;

// Bad
if (a == b) {
    c = d;
}

// Bad - inconsistent style
if (a == b)
    c = d;
else
{
    c = e;
}
```

2. Do not add blank lines between sets of closing braces:

```csharp
if (a == b)
{
    if (c == d)
    {
        if (e == f)
        {
            DoSomething();
        }
    }

} // The blank line above this brace should be removed.
```

3. Include blank lines after closing braces as long as the next statement isn't part of a continuing language construct (e.g., if / else, try / catch / finally):

```csharp
// Ok
public static string ToCamelCase(string text)
{
    if (string.IsNullOrWhiteSpace(text))
    {
        return text;
    }

    return char.ToLower(text[0]) + text.Substring(1);
}

// Acceptable
public static string ToCamelCase(string text)
{
    if (string.IsNullOrWhiteSpace(text))
        return text;

    return char.ToLower(text[0]) + text.Substring(1);
}

// Bad
public static string ToCamelCase(string text)
{
    if (string.IsNullOrWhiteSpace(text))
    {
        return text;
    }
    return char.ToLower(text[0]) + text.Substring(1);
}
```

4. Avoid unnecessary parentheses in expressions.

5. When writing a method and your line will exceed line length limit, consider adding a line break for each parameter (first one included).

```csharp
public void SomeMethodWithLongSignature(
    SomeObject parameter1,
    SomeOtherObject parameter2,
    SomeTestObject parameter3,
    SomeFinalObject parameter4)
{
}
```

6. When building multi-line conditional statements, put the conditional operator at the beginning of each line:

```csharp
// Ok
public bool SomeMethod()
{
    return !(myVar || myOtherVar
        || something || somethingElse)
        && string.IsNullOrWhiteSpace(myStr)
        && !string.IsNullOrWhiteSpace(myOtherStr);
}
  
// Bad
public bool SomeMethod()
{
    return !(myVar || myOtherVar ||
        something || somethingElse) &&
        string.IsNullOrWhiteSpace(myStr) &&
        !string.IsNullOrWhiteSpace(myOtherStr);
}
```

7. Put auto-properties on a single line:

```csharp
// Ok
public int Property { get; set; }

// Bad
public int Property
{
    get;
    set;
}
```

8. Put calls to `base` or `this` constructors onto an indented separate line:

```csharp
public SomeClass(string parameter)
    : base(parameter)
{
}
```

9. Constructors with no bodies are not shortened:

```csharp
public SomeClass()
{
}

public SomeClass(string parameter)
    : base(parameter)
{
}
```

10. Bring constraints for generic types onto separate lines (indented).

```csharp
public abstract class SomeBase<TArg1, TArg2> : SomeClass
    where TArg1 : ISomething, new()
    where TArg2 : class
{
}
```

11. Namespace imports should be specified at the top of the file, outside of namespace declarations and should be sorted alphabetically, with System. namespaces at the top.

12. Class artifacts should be organized as follows (in some cases you can pack things that goes together rather than splitting to respect the following order):

    a. Fields

    b. Constructor(s)

    c. Public members (properties and methods)

    d. Define property-backing fields immediately before the property:

    ```csharp
    private string _name;

    public string Name
    {
        get => _name;
        set => _name = value;
    }
    ```

    e. Define non-shared supporting methods immediately following the method they were introduced to support, or use a local method.
    For local function prefer declaring them at the end of the method in a region named "Local function" or "Local functions".

    ```csharp
    public void DoSomethingInteresting()
    {
        // ...
        int something = GetSomething();
        // ...
        int anotherThing = GetAnotherThing();
    }

    private int GetSomething()
    {
       // ...
    }

    private int GetAnotherThing()
    {
       // ...
    }

    // Local method variant
    public void DoSomethingCompletelyDifferent()
    {
        int something = GetSomething();
        // ...

        #region Local function

        int GetSomething()
        {
            // ...
        }

        #endregion
    }
    ```

## Data Types

1. Use idiomatic C# types rather than .NET Framework types.

| Idiomatic <img src="../../images/check.svg" width="20" height="20" /> | Framework <img src="../../images/cross.svg" width="20" height="20" /> |
| --- | --- |
| string | String |
| object | Object |
| int | Int32 |
| double | Double |

2. String Data Type

    a. Use string [interpolation](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/tokens/interpolated) to concatenate short strings, as shown in the following code:

    ```csharp
    string displayName = $"{person.LastName}, {person.FirstName}";
    ```

    b. To append strings in loops, especially with a large amount of text, use a `StringBuilder` object.

    ```csharp
    string phrase = "A long sentence";
    var manyPhrases = new StringBuilder();
    for (int i = 0; i < 10000; ++i)
    {
        manyPhrases.Append(phrase);
    }
    ```

     c. Use case-insensitive checks rather than converting the casing of strings for case-sensitive comparison and generally consider specifing `StringComparison` explicitly.

     ```csharp
    string str1 = "Some string value";
    string str2 = "SoMe STRing ValuE";

    // Ok
    if (str1.Equals(str2, StringComparison.InvariantCultureIgnoreCase))
    {
        return true;
    }

    // Ok
    if (string.Equals(str1, str2, StringComparison.InvariantCultureIgnoreCase))
    {
        return true;
    }

    // Bad
    if (value1.ToLower() == value2.ToLower())
    {
        return true;
    }
    ```

3. Always specify type of local variables for idiomatic types (int, string, etc.). But consider using `var` to avoid type duplication when variable type is visible in the statement:

```csharp
// When the type of a variable is clear from the context, use var otherwise explicit it
string var1 = "A string";
int var2 = 27;
int var3 = Convert.ToInt32(Console.ReadLine());
int var4 = SomeFunction();
var var5 = new SomeClass();
SomeClass var6 = SomeOtherFunction();
```

## Initialization

1. Use the concise syntax for arrays when you initialize them on the declaration line:

```csharp
string[] vowels1 = { "a", "e", "i", "o", "u" };
 
// If you use explicit instantiation, you can use var.
var vowels2 = new string[] { "a", "e", "i", "o", "u" };
 
// If you specify an array size, you must initialize the elements one at a time.
var vowels3 = new string[5];
vowels3[0] = "a";
vowels3[1] = "e";
// Etc
```

2. Use object initializers to simplify object creation:

```csharp
// Ok
var myObjects = new List<SomeObject>
{
    new SomeObject("value1"),
    new SomeObject("value2"),
};

var studentById = new Dictionary<int, Student>
{
    [123456] = new Student { Name = "John Doe", Age = 17} },
    [456789] = new Student { Name = "Dany Doe", Age = 16} },
};
```

## Exception Handling

1. Use a try-catch statement for most exception handling.

2. Simplify code with `IDisposable` object by using the C# `using` statement. If you have a try-finally statement in which the only code in the finally block is a call to the `Dispose` method.

```csharp
// This try-finally statement only calls Dispose in the finally block
var someDisposableObject = new DisposableObject();
try
{
    // ...
}
finally
{
    someDisposableObject.Dispose();
}
 
// You can do the same thing with a using statement
using (var someDisposableObject = new DisposableObject())
{
    // ...
}
```

3. Define custom exception classes when you expect the exception to be explicitly handled. It's a lot more reliable to match an exception based on its type than by extracting information from the message.

## Null Detection & Check

It is a good advice to avoid null reference exceptions by checking for null values before using an object that may be null.
Also it is good to make checks early in public API to avoid broadcasting bad value to internal systems.

1. On public, protected, and internal method arguments consider doing check:

```csharp
public void Something(object obj)
{
    if (obj is null)
        throw new ArgumentNullException(nameof(obj));
}
```

2. On constructor arguments:

```csharp
public class A
{
    private readonly B _b;

    public A(B b)
    {
        _b = b ?? throw new ArgumentNullException(nameof(b));
    }
}
```

3. When doing null check prefer using `is null` rather than `== null` when possible.

## LINQ Queries

1. When using LINQ prefer using [Method Syntax over the Query Syntax for LINQ](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/query-syntax-and-method-syntax-in-linq).

2. Use meaningful names for query variables. The following example uses `tolkienBooks` for books written by Tolkien.

```csharp
var tolkienBooks = books.Where(x => x.Author == "Tolkien")
                        .Select(x => x.Name);
```

3. Use aliases to make sure that property names of anonymous types are correctly capitalized, using Pascal casing.

```csharp
var booksInfo = books.Join(
    authors,
    book => book.Author,
    author => author.Name,
    (book, author) => new
    {
        Book = book,
        Author = author
    });
```

4. Rename properties when the property names in the result can be ambiguous. For example, if your query returns a book name and an author ID, instead of leaving them as Name and Id in the result, rename them to clarify that Name is the name of a book, and ID is the ID of an author.

```csharp
var books = books.Join(
    authors,
    book => book.Author,
    author => author.Name,
    (book, author) => new
    {
        BookName = book.Name,
        AuthorId = author.Id
    });
```

5. Except when too long, prefer using explicit typing in the declaration of query variables and range variables.

6. Use [Where](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/where-clause) clauses before other query clauses to ensure that later query clauses operate on the reduced, filtered set of data.

```csharp
var tolkienBooks = books.Where(x => x.Author == "Tolkien")
                        .OrderBy(x => x.PublicationYear)
                        .Select(x);
```

## Other Rules

1. Always use explicit scope/visibility.

```csharp
// Ok
public class Something
{
    private const int MaximumAge = 100;

    public int Age { get; set; }

    public void SayHello()
    {
        Console.WriteLine("Hello");
    }
}

// Bad
class Something
{
    const int MaximumAge = 100;

    int Age { get; set; }

    void SayHello()
    {
        Console.WriteLine("Hello");
    }
}
```

2. As much as possible, try to have only one class per file. It is not a problem if you end up with several files containing only a few lines. Of course, this does not apply to inner classes.

3. Use `string.Empty` rather than empty quotes (`""`) for empty strings when possible.

4. Avoid methods that exceed 20-30 lines of code. After that, consider refactoring the method into smaller well-named methods.

5. When using local functions:

    a. Try to limit the usage of captured variables from the enclosing method, as they can make it difficult to read and understand the overall method.

    b. Limit local methods to only a few lines of code, to make overall method easier to understand.

6. Prefer adopting a coding style of "fail fast" and "exit fast". If there is an exception to be thrown or a value to be returned with no further logic, perform that logic immediately rather than leaving it for an `else` clause.

```csharp
// Ok
public void SomeMethod(ISomething something)
{
    // Check for known failure condition and exit quickly
    if (something is null)
        throw new ArgumentNullException(nameof(something));

    var things = _somethingElse.GetThoseThings();
    // ...
}

// Bad
public void SomeMethod(ISomething something)
{
    if (something != null)
    {
        var things = _somethingElse.GetThoseThings();
        // Lots of logic here that a maintainer has to scroll through.
    }
    else
    {
        // This should be moved to the top of the method
        throw new ArgumentNullException(nameof(something));
    }
}
```

7. Avoid the use of the `dynamic` keyword, particularly in code that executes frequently. Dynamic dispatch is very expensive and is rarely needed.

8. Avoid passing values around using `KeyValuePair<TKey, TValue>`, but if you do, use a name format like `{keyName}And{valueName}` to ease maintenance (for example use `studentIdAndName` for an entry from a dictionary named `studentNameById`).

9. Use explicit property name on anonymous types, and prefer placing each property on a separate line.

```csharp
var anonymousObj =
{
    Property1 = sourceA.Property1,
    Property2 = sourceB.Property2
};
```

10. If you are using ReSharper consider keeping files warning free. Follow its guidelines, and suppress warning if relevant with a justification.

11. Consider putting the maximum number of ReSharper annotations. This add usefull suggestions for developers using ReSharper and also allow to quickly determine contracts.
Annotations that are the most used are the following, others are not mandatory:
    - NotNull
    - CanBeNull
    - ItemNotNull
    - ItemCanBeNull
    - Pure
    - InstantHandle
    - ContractAnnotation
    - UsedImplicitly (more rarely)