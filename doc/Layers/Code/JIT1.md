# Just-In-Time Compilation: Tier1

[WIKI](https://en.wikipedia.org/wiki/Just-in-time_compilation)


Read the [Source](https://github.com/dotnet/runtime/tree/main/src/coreclr/jit), in C++, as part of the CoreCLR Runtime

The first pass, is fast, simple and designed to get the app running quickly.

Hot-paths, or frequently used methods, get a second optimised JIT.

Matt Warrens [.NET JIT and CLR - Joined at the Hip](https://mattwarren.org/2018/07/05/.NET-JIT-and-CLR-Joined-at-the-Hip/)

Runtime Documentation:
 - [Index](https://github.com/dotnet/runtime/tree/main/docs/design/coreclr/jit)
 - [Ryujit](https://github.com/dotnet/runtime/blob/main/docs/design/coreclr/jit/ryujit-overview.md)


