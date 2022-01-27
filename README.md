# DependencyCop

This repository contains an implementation of a number of Roslyn analyzer rules using the .NET Compiler Platform. The rules enforce certain restrictions on dependencies between code in different namespaces.

For an overview of the rules, see [README.md](./Liversen.DependencyCop/README.md).

## Using DependencyCop

The severity of individual rules may be configured using [rule set files](https://docs.microsoft.com/en-us/visualstudio/code-quality/using-rule-sets-to-group-code-analysis-rules).

Rule [DC1001](https://github.com/larsiver/DependencyCop/blob/main/Liversen.DependencyCop/Documentation/DC1001.md) requires additional configuration to be enabled, see the documentation for that rule for further info.

## Installation

The analyzers can be installed using the NuGet command line or the NuGet Package Manager in Visual Studio.

Install using the command line:

    Install-Package Liversen.DependencyCop