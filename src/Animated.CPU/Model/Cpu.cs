using System;
using System.Collections.Generic;
using System.Globalization;
using Animated.CPU.Primitives;

namespace Animated.CPU.Model
{

    

    // https://software.intel.com/content/www/us/en/develop/articles/introduction-to-x64-assembly.html
    public class Cpu 
    {
        public Cpu()
        {
            RegisterFile = new List<Register>()
            {
                RIP,
                RFLAGS,
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
                CLK
                
            };
            Main = new Memory((int)Math.Pow(2, 16));
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
        
        public Register RIP    { get; } = new Register("RIP", "Instruction Pointer");
        public Register RFLAGS { get; } = new Register("RFLAGS", "Flags");
        public Register CLK { get; } = new Register("CLK", "Clock");

        public List<Register> RegisterFile { get; }
        
        public IMemory L1           { get; set; }
        public IMemory L2           { get; set; }
        public IMemory L3           { get; set; }
        public IMemory Main         { get; set; }
        
        
        public MemoryView Instructions { get; set; }
        public MemoryView Stack { get; set; }

        public ArithmeticLogicUnit   ALU             { get; set; } = new ArithmeticLogicUnit();
        
        
        // Animation & Story Replay
        public Story Story { get; set; }


        public void Step()
        {
            CLK.Value++;
        }

        public Register? SetReg(string name, ulong value)
        {
            foreach (var register in RegisterFile)
            {
                if (string.Equals(register.Id, name, StringComparison.InvariantCultureIgnoreCase))
                {
                    
                        if (register.SetValue(value))
                        {
                            return register;
                        }
                        
                    
                    return null;
                }
            }
            return null;
        }
    }

    public class ArithmeticLogicUnit
    {
        public object                Fetch   { get; set; } = "Fetch";
        public object                Decode  { get; set; } = "Decode";
        public object                Execute { get; set; } = "Execute";
        public object                Step    { get; set; } = "Step";

        public IEnumerable<object> Phases()
        {
            yield return Fetch;
            yield return Decode;
            yield return Execute;
            yield return Step;
        }
    }

    

    
    
    public class Register : Prop<ulong>
    {
        public Register(string id, string name) : base(PropHelper.DefaultCompareULong, 0, null)
        {
            Id   = id;
            Name = name;
        }

        public string Id          { get; set; }
        public string Name        { get; set; }
        public string Description { get; set; }

        public string ValueHex => Value.ToString("X").PadLeft(64 / 8 * 2, '0');
        

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

  


}
