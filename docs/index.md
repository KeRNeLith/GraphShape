# GraphShape documentation

## Badges

| | |
| --- | --- |
| **Build** | [![AppVeyor Build Status](https://ci.appveyor.com/api/projects/status/github/KeRNeLith/GraphShape?branch=master&svg=true)](https://ci.appveyor.com/project/KeRNeLith/GraphShape) |
| **Coverage** | <sup>Coveralls</sup> [![Coverage Status](https://coveralls.io/repos/github/KeRNeLith/GraphShape/badge.svg?branch=master)](https://coveralls.io/github/KeRNeLith/GraphShape?branch=master) <sup>SonarQube</sup> [![SonarCloud Coverage](https://sonarcloud.io/api/project_badges/measure?project=graphshape&metric=coverage)](https://sonarcloud.io/component_measures/metric/coverage/list?id=graphshape) | 
| **Quality** | [![Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=graphshape&metric=alert_status)](https://sonarcloud.io/dashboard?id=graphshape) | 
| **License** | [![GitHub license](https://img.shields.io/github/license/mashape/apistatus.svg)](https://github.com/KeRNeLith/GraphShape/blob/master/LICENSE) |

## Introduction

**GraphShape** is a .NET library that mainly provide graph layout framework.
It contains several overlap removal and layout algorithms that allow various kind of layouts and has also a module with customizable controls for WPF applications visualization.

Based on the [GraphSharp](https://archive.codeplex.com/?p=graphsharp) library, it has be reworked add some improvements to it.

Main features:
- Several layout algorithms (FR, KK, ISOM, LinLog, Simple Tree, Circular, Sugiyama, Compound FDP, Random).
- Overlap removal algorithms (FSA or One Way FSA).
- Customizable WPF controls for visualization.

## Targets

- .NET Standard 2.0+
- .NET Core 2.0+
- .NET Framework 3.5+

Supports Source Link

## Notes

- It uses NUnit3 for unit testing (not published).

- The library code is published annotated with JetBrains annotations.

## Packages

GraphShape is available on [NuGet](https://www.nuget.org) in several modules.

[![Nuget Status](https://img.shields.io/nuget/v/graphshape.svg)](https://www.nuget.org/packages/GraphShape) [GraphShape](https://www.nuget.org/packages/GraphShape) (Core)

    PM> Install-Package GraphShape

[![Nuget Status](https://img.shields.io/nuget/v/graphshape.controls.svg)](https://www.nuget.org/packages/GraphShape.Controls) [GraphShape.Controls](https://www.nuget.org/packages/GraphShape.Controls)

    PM> Install-Package GraphShape.Controls

<img src="images/graphshape_logo.png" width="128" height="128" style="display: block; margin-left: auto; margin-right: auto" />