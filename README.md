# DependencyCop

This repository contains an implementation of a number of Roslyn analyzer rules using the .NET Compiler Platform. The rules enforce certain restrictions on dependencies between code in different namespaces.

For an overview of the rules, see [README.md](./Liversen.DependencyCop/README.md).

## Rationale

> Identifier naming is important when you write code. That applies to all kinds of identifiers such as namespaces, classes, functions and variables. Identifiers that only exist in smaller contexts such as local variables inside small functions can be short and require less consideration than other names... Namespace names exist in very large contexts, thus you should take great care when you name them.

The above quote is from [Namespace Naming](https://www.linkedin.com/pulse/namespace-naming-lars-iversen/). The key point is that good programming starts with well-chosen names including namespace names. And those namespaces have to be carefully structured as they are an important part of many (low level) software architectures.

The rules in this repository aim at helping getting those namespace structures right by applying some restrictions to what can be done. As soon as a software project grows and the number of namespaces increases, these rules are a first-line defence against poor architecture. 

## Using DependencyCop

The severity of individual rules may be configured using [rule set files](https://docs.microsoft.com/en-us/visualstudio/code-quality/using-rule-sets-to-group-code-analysis-rules).

Rule [DC1001](https://github.com/larsiver/DependencyCop/blob/main/Liversen.DependencyCop/Documentation/DC1001.md) requires additional configuration to be enabled, see the documentation for that rule for further info.

## Installation

The analyzers can be installed using the NuGet command line or the NuGet Package Manager in Visual Studio.

Install using the command line:

    Install-Package Liversen.DependencyCop