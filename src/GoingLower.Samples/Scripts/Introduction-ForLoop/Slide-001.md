# Register Names

The easiest way to understand the register names its history:

x86 family journey:
- 1976: Intel [8086](https://en.wikipedia.org/wiki/Intel_8086) 16-bit chip, is the origin of the "x86" name
- 1985: Intel [80386](https://en.wikipedia.org/wiki/Intel_80386) 32-bit chip.
- 2000: AMD [AMD-64](https://en.wikipedia.org/wiki/X86-64) 64-bit chip, aka x86-64.
  
> Aside: Intel's [IA-64](https://en.wikipedia.org/wiki/IA-64) was cleaner but not backwards compatible and failed in the marketplace.


Take the A register for example:
- 16-bit `AX`
- 32-bit `EAX`      
- 64-bit `RAX`

The same hold for the other registers:

                            16   32   64
- Source Index        (SI): SI, ESI, RSI
- Destination Index   (DI): DI, EDI, RDI
- Instruction Pointer (IP): IP, EIP, RIP
- Stack Pointer       (SP): SP, ESP, RSP
- A Register          (A):  AX, EAX, RAX
- B Register          (B):  BX, EBX, RBX
- C Register          (C):  CX, ECX, RCX
- D Register          (D):  DX, EDX, RDX



























TODO: Diagram with die-size, clock speed, and transistor count
   