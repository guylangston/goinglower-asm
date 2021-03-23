# <Slide> Welcome

Topic: C# for-loop
CPU: Intel x86 CPU, 64-bit mode

## History

A CPU's register size covers both addressing (memory pointers) and general calculations (ADD, MUL, etc). 
Conceptually, it is easier of they are the same size, but they don't need to be. 

x86 family journey:
- 16-bit [8086](https://en.wikipedia.org/wiki/Intel_8086) chip in 1976; which is we the x86 comes from
- 32-bit [80386](https://en.wikipedia.org/wiki/Intel_80386) chip in 1985
- 64-bit [AMD-64 aka x86-64](https://en.wikipedia.org/wiki/X86-64) design by AMD (not Intel) in 2000 for the AMD K8 chips. 
  Intel's cleaner but not bdackwards compatible IA-64 effectively failed in the marketplace.
- After 64-bit we got special purpose 128-bit computation with MMX and onwards. (These are out of scope now)

TODO: Diagram with die-size, clock speed, and transistor count

x86-64 allows 64-bit addresses @RIP but 32-bit general registers @EAX, @EBX, etc. This is effectively the dotnet model.


# <Slide> The Journey: Going Lower

> How did we get from .CS file to OpCode in Memory?

            Runtime =>
C# => IL =>    JIT  => Binary Code
               DLL  =>

# <Slide> Questions?

Some interesting questions:
- What does XOR do?
- Why test edi, edi; then later cmp esi, esi (TODO: Add offsets)
- How is the value returned C# "return sum;"?
- What is the purpose of the first time "mov rbp, esp"?

# <Steps>

Step: 2
    This is part of the calling preamble, we can ignore it for now
    $Tag    edi     var count

Step: 3
    Just a fancy way of doing `mov eax, 0`
    $Tag    eax     var sum

Step: 4
    $Tag    esi     var x

Step: 5,6
    Entry Guard. If `ebp/count` is 0, then noting to do. Jump to exit

Step: 9, 10
    Exit Check.
    Topic: CompareFlags
    




