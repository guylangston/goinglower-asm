# GoingLower: C# to machine code

> Animates code execution  (with state and forward/undo)

- Project Home: [www.goinglower.dev](https://www.goinglower.dev)

- Videos: [YouTube](https://www.youtube.com/channel/UCvghNVWLxYU00NKxLYMYbdQ/videos)



## Project Goals:

- Animate real code, not an simplified cpu model
- Target `asm/c/c#` ; show high-level source maps
- Interpret `ASM` to pseudo code 
  
  ```nasm
  inc eax                 ; eax <- eax + 1"
  lea ebx, [rsi + 10]     ; ebx <- rsi + 10"
  xor ebx, ebx            ; ebx <- 0"
  ```

### UI Goals

- Quick OpCode lookup/help (it is too unwieldy to lookup all the OpCodes from the Intel Handbook)

## Inspiration:

- the book "xpord",
- [This Goes to Eleven (Part 1/∞) - damageboy](https://bits.houmus.org/2020-01-28/this-goes-to-eleven-pt1) by [@damageboy](https://twitter.com/damageboy)
- [Bartosz Adamczewski @badamczewski01](https://twitter.com/badamczewski01)

# Getting Started
```shell
$ git clone https://github.com/guylangston/goinglower-asm.git
$ cd gowinglower-asm
$ cd src/GoingLower.UI.GTK
$ ./run.sh
```
