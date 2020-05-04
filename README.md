| | |
| --- | --- |
| **Build** | [![AppVeyor Build Status](https://ci.appveyor.com/api/projects/status/github/KeRNeLith/GraphShape?branch=master&svg=true)](https://ci.appveyor.com/project/KeRNeLith/GraphShape) |
| **Coverage** | <sup>Coveralls</sup> [![Coverage Status](https://coveralls.io/repos/github/KeRNeLith/GraphShape/badge.svg?branch=master)](https://coveralls.io/github/KeRNeLith/GraphShape?branch=master) <sup>SonarQube</sup> [![SonarCloud Coverage](https://sonarcloud.io/api/project_badges/measure?project=graphshape&metric=coverage)](https://sonarcloud.io/component_measures/metric/coverage/list?id=graphshape) | 
| **Quality** | [![Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=graphshape&metric=alert_status)](https://sonarcloud.io/dashboard?id=graphshape) | 
| **Nugets** | [![Nuget Status](https://img.shields.io/nuget/v/graphshape.svg)](https://www.nuget.org/packages/GraphShape) GraphShape |
| | [![Nuget Status](https://img.shields.io/nuget/v/graphshape.svg)](https://www.nuget.org/packages/GraphShape.Controls) GraphShape.Controls |
| **License** | [![GitHub license](https://img.shields.io/github/license/mashape/apistatus.svg)](https://github.com/KeRNeLith/GraphShape/blob/master/LICENSE) |

# GraphShape

## What is **GraphShape**?

**GraphShape** is a .NET library that mainly provide graph layout framework.
It contains several overlap removal and layout algorithms that allow various kind of layouts and has also a module with customizable controls for WPF applications visualization.

Based on the [GraphSharp](https://archive.codeplex.com/?p=graphsharp) library, it has be reworked add some improvements to it.

Main features:
- Several layout algorithms (FR, KK, ISOM, LinLog, Simple Tree, Circular, Sugiyama, Compound FDP).
- Overlap removal algorithms (FSA or One Way FSA).
- Customizable WPF controls for visualization.

See the library [documentation](https://kernelith.github.io/GraphShape/).

---

## Targets

- [![.NET Standard](https://img.shields.io/badge/.NET%20Standard-%3E%3D%202.0-blue.svg)](#)
- [![.NET Core](https://img.shields.io/badge/.NET%20Core-%3E%3D%202.0-blue.svg)](#)
- [![.NET Framework](https://img.shields.io/badge/.NET%20Framework-%3E%3D%203.5-blue.svg)](#)

Supports Source Link (use dedicated symbol package)

To get it working you need to:
- Uncheck option "Enable Just My Code"
- Add the NuGet symbol server (*https://symbols.nuget.org/download/symbols*)
- Check option "Enable Source Link support"

---

## Notes

- It uses NUnit3 for unit testing (not published).

- The library code is published annotated with JetBrains annotations.

---

## Usage

### Packages

GraphShape is available on [NuGet](https://www.nuget.org) in several modules.

- [GraphShape](https://www.nuget.org/packages/GraphShape) (Core)
- [GraphShape.Controls](https://www.nuget.org/packages/GraphShape.Controls)

### Where to go next?

* [Documentation](https://kernelith.github.io/GraphShape/)

---

## Maintainer(s)

* [@KeRNeLith](https://github.com/KeRNeLith)

---