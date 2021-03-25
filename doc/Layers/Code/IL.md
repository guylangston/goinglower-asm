# Dotnet Intermediate Language (IL)_

> Common Intermediate Language (CIL), formerly called Microsoft Intermediate Language (MSIL) or Intermediate Language (IL). 
> CIL is object-oriented, stack-based bytecode.
> CIL is a CPU- and platform-independent instruction set 

[WIKI](https://en.wikipedia.org/wiki/Common_Intermediate_Language)


[Specification](https://www.ecma-international.org/publications-and-standards/standards/ecma-335/)


## Hello World in `IL`
```
.assembly Hello {}
.assembly extern mscorlib {}
.method static void Main()
{
     .entrypoint
     .maxstack 1
     ldstr "Hello, world!"
     call void [mscorlib]System.Console::WriteLine(string)
     ret
}
```
[Source](https://en.wikibooks.org/wiki/Computer_Programming/Hello_world#CIL)
