# GoingLower

> Animated execution of C# down to machine code on modern hardware

Goals:
- Animate real code, not an simplified cpu model
- Target asm/c/c# ; show high-level source maps
- Interpret ASM to pseudo code 
   - "inc eax                 ; eax <- eax + 1"
   - "lea ebx, [rsi + 10]     ; ebx <- rsi + 10"
   - "xor ebx, ebx            ; ebx <- 0"
  
UI Goals
- Quick OpCode lookup/help (it is too unwieldy to lookup all the OpCodes from the Intel Handbook)

Inspiration:
- the book "xpord",
- @damageboy
- Bartosz Adamczewski @badamczewski01
