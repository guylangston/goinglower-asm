# Intel x86 Register Names

```
RAX = 64 bit     'A' Register      Mask: FF:FF:FF:FF--FF:FF:FF:FF
EAX = 32 bit     'A' Register      Mask: 00:00:00:00--FF:FF:FF:FF
 AX = 16 bit     'A' Register      Mask: 00:00:00:00--00:00:FF:FF
 AH = 8 bit HIGH 'A' Register      Mask: 00:00:00:00--00:00:FF:00
 AL = 8 bit LOW  'A' Register      Mask: 00:00:00:00--00:00:00:FF
```

The same pattern holds for general purpose A, B, C, D registers

In dotnet/C#:
 - addresses are  64-bit R?X,
 - general workings like `int` are 32-bit E?X
