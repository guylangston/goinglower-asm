# Register Names

The easiest way to understand the register names is via the x86 history:

x86 family journey:
- 1976: Intel  [8086](https://en.wikipedia.org/wiki/Intel_8086) 16-bit chip,
  which is we the x86 comes from
- 1985; Intel [80386](https://en.wikipedia.org/wiki/Intel_80386) 32-bit chip.
- 2000: AMD [AMD-64 aka x86-64](https://en.wikipedia.org/wiki/X86-64) 64-bit chip.
  
>  Intel's cleaner but not backwards compatible IA-64 effectively failed in the marketplace.

TODO: Diagram with die-size, clock speed, and transistor count

Take the A register for example:
- 16-bit `AX`
- 32-bit `EAX`
- 64-bit `RAX`

The same hold for the other registers:
- Instruction Pointer (IP): IP, EIP, RIP
- Stack Pointer       (SP): SP, RSP, RSP
- B Register          (B):  BX, RBP, RBP
- C Register          (C):  CX, RCP, RCP
- D Register          (D):  DX, RDP, RDP
- Source Index        (SI): SI, RSI, RSI
- Destination Index   (DI): DI, DSI, DSI
   