# Flag Register

Flags, or individual bits, are set during maths, logic and comparison operations.


```
Bit Label  Description
0   CF     Carry Flag
1   1      Reserved
2   PF     Parity Flag
3   0      Reserved
4   AF     Auxiliary Carry Flag
5   0      Reserved
6   ZF     Zero Flag
7   SF     Sign Flag
8   TF     Trap Flag
9   IF     Interrupt Enable Flag
10  DF     Direction Flag
11  OF     Overflow Flag
12  IOPL   I/O Privilege Level
13  IOPL   I/O Privilege Level
14  NT     Nested Task
15  0      Reserved
16  RF     Resume Flag
17  VM     Virtual-8086 Mode
18  AC     Alignment Check / Access Control
19  VIF    Virtual Interrupt Flag
20  VIP    Virtual Interrupt Pending
21  ID     ID Flag
	
```
[Source](https://wiki.osdev.org/CPU_Registers_x86-64) 