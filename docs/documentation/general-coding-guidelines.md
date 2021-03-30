# General Coding Guidelines

## Introduction

The coding guidelines serve the following purposes:
- They create a consistent look to the code, so that readers can focus on content, not layout.
- They enable readers to understand the code more quickly by making assumptions based on previous experience.
- They facilitate copying, changing, and maintaining the code.

## General Principles

- The guidelines should be used as a guide, they are not absolute, and exceptions are allowed when justified.
- The readability is essential:
    1. Be consistent in style
    2. Use empty lines and spaces appropriately
    3. Use descriptive names and naming conventions at all times
    4. Follow existing patterns unless there is a good reason
    5. Keep methods and classes short and simple (Single Responsibility Principle (SRP))
- Consider making small commits is order to keep changes easily understandable.

## Layout Conventions

Code formatted the same way everywhere makes the code easier to read.
Below are prefered layout settings to be used for QuikGraph.

1. Setup code editors to use four-character indents and save tab characters as spaces.
2. Write only one statement per line.
3. Write only one declaration per line.
4. Avoid more than one empty line at a time.
5. Limit the length of a line of code to about 120-140 characters to reduce unnecessary horizontal scrolling.

## Commenting Conventions

Use comments liberally to provide useful context with proper English sentences.

Insert one space between the comment delimiter and the comment text, as shown in the following example:

```csharp
// This is a example comment. It showcases
// the way comment must be written.
```

Comments generally have a blank line before and, at the very least, after the segment of code to which the comment applies (in the example it is before the `Console.WriteLine`).

```csharp
int[] scores = GetScores();

// Calculate the average
int count = scores.Count;
double sum = scores.Sum();
double average = sum / count;

Console.WriteLine(x.Average);
```

All public API must be documented using the [standard C# comments](https://docs.microsoft.com/en-US/dotnet/csharp/language-reference/language-specification/documentation-comments), and must be written at third person.
Note that public API is not limited to public members/methods but also applies to inheritable structure having protected members/methods for example.