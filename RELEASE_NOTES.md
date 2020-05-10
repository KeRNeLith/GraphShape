# Release notes

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