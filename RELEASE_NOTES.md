# Release notes

## What's new in 0.1.2 August 5 2020

### GraphShape

#### Fixes

* Fix a crash in the Efficient Sugiyama algorithm when having less thatn 2 layers.

### GraphShape.Controls

No relevant changes.

## What's new in 0.1.1 August 3 2020

### GraphShape

#### Fixes

* Fix the orthogonal edge routing of Efficient Sugiyama algorithm (in horizontal layout).

### GraphShape.Controls

No relevant changes.

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