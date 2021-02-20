using System;
using System.Collections.Generic;

namespace Animated.CPU.Model
{


    // https://software.intel.com/content/www/us/en/develop/articles/introduction-to-x64-assembly.html
    public class Cpu 
    {
        public Cpu()
        {
            RegisterFile = new List<Register>()
            {
                R0,
                R1,
                R2,
                R3,
                R4,
                R5,
                R6,
                R7,
                R8,
                R9,
                R10,
                R11,
                R12,
                R13,
                R14,
                R15,
                
                RIP,
                RFLAGS
            };

            Main = new Memory((int)Math.Pow(2, 16));

            RIP.Value = 6000;
            
        }
        
        // https://en.wikibooks.org/wiki/X86_Assembly/X86_Architecture
        public Register RAX { get; } = new Register("RAX", "Accumulator") { Description =  "Used in arithmetic operations"}; 
        public Register RBX { get; } = new Register("RBX", "Base");
        public Register RCX { get; } = new Register("RCX", "Count");
        public Register RDX { get; } = new Register("RDX", "Data");
        public Register RBP { get; } = new Register("RBP", "Stack Base Pointer");
        public Register RSI { get; } = new Register("RSI", "Source Index");
        public Register RDI { get; } = new Register("RDI", "Destination Index");
        public Register RSP { get; } = new Register("RSP", "Stack Pointer");
        

        public Register R0 => RAX;
        public Register R1 => RBX;
        public Register R2 => RCX;
        public Register R3 => RDX;
        public Register R4 => RBP;
        public Register R5 => RSI;
        public Register R6 => RDI;
        public Register R7 => RSP;
        
        public Register R8  { get; } = new Register("R8",  "Register 8");
        public Register R9  { get; } = new Register("R9",  "Register 9");
        public Register R10 { get; } = new Register("R10", "Register 10");
        public Register R11 { get; } = new Register("R11", "Register 11");
        public Register R12 { get; } = new Register("R12", "Register 12");
        public Register R13 { get; } = new Register("R13", "Register 13");
        public Register R14 { get; } = new Register("R14", "Register 14");
        public Register R15 { get; } = new Register("R15", "Register 15");
        
        public Register RIP { get; } = new Register("RIP", "Instruction Pointer");
        public Register RFLAGS { get; } = new Register("RFLAGS", "Flags");

        public List<Register> RegisterFile { get; }
        
        public IMemory L1 { get; set; }
        public IMemory L2 { get; set; }
        public IMemory L3 { get; set; }
        public IMemory Main { get; set; }

       
        
    }

    class Operation
    {
        public Instruction Instruction { get; set; }
        public Arg[]       Args        { get; set; }
    }
    
    
    
    public class Register 
    {
        public Register(string id, string name)
        {
            Id   = id;
            Name = name;
        }

        public string Id          { get; set; }
        public string Name        { get; set; }
        public string Description { get; set; }
        public ulong  Value       { get; set; }

        public override string ToString() => $"{Id}/{Name} = {Value:X}";
    }

    public interface IMemory
    {
        byte GetByte(ulong ptr);       // 8
        ushort GetWord(ulong ptr);     // 16
        uint GetDWord(ulong ptr);      // 32
        ulong GetQWord(ulong ptr);     // 64
        
        void SetByte(ulong ptr, byte val);        // 8
        void SetWord(ulong ptr, ushort val);      // 16
        void SetDWord(ulong ptr, uint val);       // 32
        void SetQWord(ulong ptr, ulong val);      // 64
    }

    public class Memory : IMemory
    {
        private byte[] block;

        public Memory(int size)
        {
            block = new byte[size];
        }


        public byte GetByte(ulong ptr) => block[ptr];
        public ushort GetWord(ulong ptr)
        {
            throw new System.NotImplementedException();
        }
        public uint GetDWord(ulong ptr)
        {
            throw new System.NotImplementedException();
        }
        public ulong GetQWord(ulong ptr)
        {
            throw new System.NotImplementedException();
        }
        
        public void SetByte(ulong ptr, byte val)
        {
            block[ptr] = val;
        }
        public void SetWord(ulong ptr, ushort val)
        {
            throw new System.NotImplementedException();
        }
        public void SetDWord(ulong ptr, uint val)
        {
            throw new System.NotImplementedException();
        }
        public void SetQWord(ulong ptr, ulong val)
        {
            throw new System.NotImplementedException();
        }
    }

    public abstract class Arg
    {
        public Register AsRegister(Cpu cpu)
        {
            return cpu.R0; // todo
        }
        
        public ulong AsImmediate()
        {
            return 123;
        }
    }

    public class ArgImmediate : Arg
    {
        public ulong Value { get; }
    }

    public class ArgRegister : Arg
    {
        public Register Register { get; }
    }

    public abstract class Instruction
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public abstract void Execute(Cpu cpu, Arg[] args);
    }
    
    public  class InstructionZeroArg : Instruction
    {
        public string Id   { get; set; }
        public string Name { get; set; }

        public override void Execute(Cpu cpu, Arg[] args)
        {
            
        }
    }
    

    public class InstructionMOV : Instruction
    {
        public InstructionMOV()
        {
            Id   = "MOV";
            Name = "Move";
        }

        public override void Execute(Cpu cpu, Arg[] args)
        {
            // dest:reg, immediate
            args[0].AsRegister(cpu).Value = args[1].AsImmediate();

        }
    }

    
}
