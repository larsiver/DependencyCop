# DependencyCop

This repository contains an implementation of a number of Roslyn analyzer rules using the .NET Compiler Platform. The rules enforce certain restrictions on dependencies between code in different namespaces.

[Rule DC1000: Code must not refer code in descendant namespaces](./Documentation/DC1000.md)

[Rule DC1001: Using namespace statements must not reference disallowed namespaces](./Documentation/DC1001.md)

[Rule DC1002: Code must not contain namespace cycles](./Documentation/DC1002.md)
