# Flag Register

In this context, flags means each bit represents a independent on/off state.
They are set during comparisons (as well as logic and maths ops). 
https://wiki.osdev.org/CPU_Registers_x86-64 

Bit Label  Description
0   CF     Carry
1          <Reserved>
2   PF     Parity 
3          <Reserved>
4   AF     Auxiliary Carry 
5          <Reserved>
6   ZF     Zero 
7   SF     Sign 
8   TF     Trap 
9   IF     Interrupt Enable 
10  DF     Direction 
11  OF     Overflow 

Advanced:
12  IOPL   I/O Privilege Level
13  IOPL   I/O Privilege Level
14  NT     Nested Task
15         <Reserved>
16  RF     Resume 
17  VM     Virtual-8086 Mode
18  AC     Alignment Check / Access Control
19  VIF    Virtual Interrupt 
20  VIP    Virtual Interrupt Pending
21  ID     ID 
	
 