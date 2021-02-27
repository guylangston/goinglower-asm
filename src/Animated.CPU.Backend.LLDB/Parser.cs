using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
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
        
        public static ulong ParseHexWord(string txt) => ulong.Parse(txt, NumberStyles.HexNumber);
        public static byte[] ParseHexByteArray(string txt) => Convert.FromHexString(txt);

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
                Address   =  ParseHexWord(arr),
                Raw       =  ParseHexByteArray(bytes),
                SourceAsm = ll[38..],
                Anchor    = source.FindAnchor(currSource)
            };
        }


        public IEnumerable<RegisterDelta> ParseRegisters(IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                if (StringHelper.TrySplitExclusive(line, " = ", out var r))
                {
                    yield return new RegisterDelta()
                    {
                        Register     = r.l.Trim(),
                        ValueString  = r.r.Trim(),
                        ValueParsed = LossyParseULong(r.r.Trim())
                    };
                }
            }
        }
        
        public static ulong? LossyParseULong(string txt)
        {
            if (txt.StartsWith("0x"))
            {
                txt = txt.Remove(0, 2);
                if (ulong.TryParse(txt, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var c))
                {
                    return c;
                }
            }
            
            if (ulong.TryParse(txt, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var a))
            {
                return a;
            }
            
            if (ulong.TryParse(txt, NumberStyles.Number, CultureInfo.InvariantCulture, out var b))
            {
                return b;
            }

            return null;
        }



        public StoryStep ParseStep(string[] readAllLines)
        {
            var clean = readAllLines[0].Remove(0, 8).TrimEnd(')');
            if (StringHelper.TrySplitExclusive(clean, ", ", out var res))
            {
                return new StoryStep()
                {
                    RIP = ParseHexWord(res.l),
                    Asm = res.r,
                    Delta = ParseRegisters(readAllLines.Skip(5)).ToImmutableArray()
                };    
            }
            return null;
        }
        
        
    }

}