# Release notes

## What's new in 1.2.2 March 19 2022

### GraphShape.Controls

#### Fixes:
* Fix the control flickering when changing the bound graph.

---

## What's new in 1.2.1 October 28 2021

### GraphShape

#### Fixes:
* Fix Sugiyama orthogonal edge routing on horizontal layout.

### GraphShape.Controls

* Update package dependencies.

---

## What's new in 1.2.0 March 31 2021

### GraphShape

#### New:
* Add support of .NET Core 3.1+.
* Add support of .NET 5+.

#### Updates:
* Update package dependencies.

### GraphShape.Controls

#### New:
* Add support of .NET Core 3.1+.
* Add support of .NET 5+.

---

## What's new in 1.1.0 June 7 2020

### GraphShape

#### New:
* Use signing key to strong name library assemby.

### GraphShape.Controls

#### New:
* Use signing key to strong name library assemby.

---

## What's new in 1.0.0 May 12 2020

### GraphShape

#### General:
* Fully clean the library code.
* Extend support of the library to .NET Framework 3.5+.
* Extend support of the library to .NET Standard 2.0+.
* Uniformize APIs and behaviors of algorithms implementations.

#### Fixes:
* Various fixes for layout algorithms implementations.
* Various fixes across the library.

#### Update:
* Algorithm parameters classes are equatable.
* Algorithm cancellation will stop run earlier if possible.

#### New:
* Add a Random layout algorithm implementation.

#### API Breaks
* Some public API breaks (but should remain simple to do a migration).
* Replace .NET framework base structs (Point, Vector, Size, Rect and Thickness) by equivalent ones to support .NET Standard.
* Some algorithm were not working well and has been removed (Sugiyama).
* Rename EfficientSugiyama to Sugiyama.
* Namespaces simplifications.

#### Misc:
* Use JetBrains annotations all over the library as much as possible.

### GraphShape.Controls

#### General:
* Fully clean the library code.
* Extend support of the library to .NET Framework 3.5+.
* Remove dependency to WPFExtensions package.

#### Fixes:
* Fix async compute mode on GraphLayout control.
* Fix handlers registrations/unregistrations.
* Various fixes across the library.

#### Misc:
* Use JetBrains annotations all over the library as much as possible.

---

## What's new in 0.1.2 August 5 2020

### GraphShape

#### Fixes:
* Fix a crash when using Efficient Sugiyama algorithm (with a graph having less than 2 layers).

### GraphShape.Controls

* Update package dependencies.

---

## What's new in 0.1.1 August 3 2020

### GraphShape

#### Fixes:
* Fix the orthogonal edge routing of Efficient Sugiyama algorithm (in horizontal layout).

### GraphShape.Controls

* Update package dependencies.

---

## What's new in 0.1.0 March 9 2020

This release is based on GraphSharp from CodePlex.

### GraphShape

Graph layout algorithms implementations.

Split GraphSharp package into 2 packages:
- GraphShape
- GraphShape.Controls

### GraphShape.Controls

WPF Controls to display graph using layout algorithms.

### API Breaks
* Namespace renamed to GraphShape (but should be easy to migrate).

### Misc:
* Generate a documentation for the library via DocFX.