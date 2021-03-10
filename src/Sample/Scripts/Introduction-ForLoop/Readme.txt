      #####                         #                                   
      #     #  ####  # #    #  ####  #        ####  #    # ###### #####  
      #       #    # # ##   # #    # #       #    # #    # #      #    # 
      #  #### #    # # # #  # #      #       #    # #    # #####  #    # 
      #     # #    # # #  # # #  ### #       #    # # ## # #      #####  
      #     # #    # # #   ## #    # #       #    # ##  ## #      #   #  
       #####   ####  # #    #  ####  #######  ####  #    # ###### #    # 

-----------------------------------------------------------------------------------

 Build your intuition for how your code execute on silicon.



  ____  _  _      __                 _                   
 / ___|| || |_   / _| ___  _ __     | | ___   ___  _ __  
| |  |_  ..  _| | |_ / _ \| '__|____| |/ _ \ / _ \| '_ \ 
| |__|_      _| |  _| (_) | | |_____| | (_) | (_) | |_) |
 \____||_||_|   |_|  \___/|_|       |_|\___/ \___/| .__/ 
                                                  |_|  

In this case we are reviewing:
    C# for-loop executing on an x86 chip ix x64-bit mode.
































We will see the machine code for the highlevel language. To get the machine code, 
we have compiled the .CS file to IL-Code for the dotnet virtual machine. On 
execution the IL-Code (aka byte code) is JIT compiled (Just in Time compilation)
into machine code. We have run a disasmbler to 'understand' the machine code in 
ASM (Assembly language, Intel-flavour).

Some interesting questions:
- What does XOR do?
- Why test edi, edi; then later cmp esi, esi (TODO: Add offsets)
- How is the value returned C# "return sum;"?
- What is the purpose of the first time "mov rbp, esp"? 
