using System;
using System.Collections.Generic;
using System.Globalization;
using Animated.CPU.Model;

namespace Animated.CPU.Backend.LLDB
{
    public class Parser
    {
        private readonly SourceProvider source;
        
        public Parser(SourceProvider source)
        {
            this.source = source;
        }

        public IEnumerable<MemoryView.Segment> ParseCLRU(IReadOnlyList<string> lines)
        {
            // 1. Scan to "Begin 00007FFF7DD360B0, size 82"
            // 2. if line starts with path => Capture SourceMap
            // 3. Append instructions until empty line

            var i = 0;
            while (i < lines.Count && !lines[i].StartsWith("Begin "))
            {
                i++;
            }
            if (i >= lines.Count) throw new Exception("Bad Parse");

            var minLengh = "00007fff7dd360ff 8b45e4               mov     eax".Length;

            string currSource = null;
            uint   offset     = 0;
            while (i < lines.Count)
            {
                var ll = lines[i];
                if (ll.StartsWith("/"))
                {
                    currSource = ll;
                }
                else if (ll.Length >= minLengh && char.IsLetterOrDigit(ll[0]))
                {
                    var inst = ParseInstruction(ll, currSource);
                    if (inst != null)
                    {
                        inst.Offset =  offset;
                        offset      += (uint)inst.Raw.Length;
                        yield return inst;
                    }
                }
                i++;
            }
        }
        
        MemoryView.Segment ParseInstruction(string ll, string currSource)
        {
            //---------|---------|---------|---------|---------|---------|
            //012345678901234567890123456789012345678901234567890123456789
            //00007fff7dd360ff 8b45e4               mov     eax

            var bytes = ll[17..37].Trim();
            var arr   = ll[0..16];
            return new MemoryView.Segment()
            {
                Address   = ulong.Parse(arr, NumberStyles.HexNumber),
                Raw       = Convert.FromHexString(bytes),
                SourceAsm = ll[38..],
                Anchor    = source.FindAnchor(currSource)
            };
        }
        
         
        public void ParseRegisters(Cpu cpu, IReadOnlyList<string> lines)
        {
            foreach (var line in lines)
            {
                if (StringHelper.TrySplitExclusive(line, " = ", out var r))
                {
                    var reg = cpu.SetReg(r.l.Trim(), r.r.Trim());
                    if (reg != null)
                    {
                        var diff = reg.Value > reg.Prev
                            ? reg.Value - reg.Prev
                            : reg.Prev - reg.Value;
                        Console.WriteLine($"\t{reg.Id,8}: {reg.Prev.ToString("X"),16} -> {reg.Value.ToString("X"),16} (diff: {diff.ToString("X"),16})");
                    }
                }
            }
        }
    }
}