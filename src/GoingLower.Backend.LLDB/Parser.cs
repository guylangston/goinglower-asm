using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using GoingLower.Core.Helpers;
using GoingLower.CPU.Model;

namespace GoingLower.Backend.LLDB
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

            string           currSource = null;
            SourceFileAnchor last       = null;
            uint             offset     = 0;
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

                        if (inst.SourceAnchor != null)
                        {
                            last = inst.SourceAnchor;
                        }
                        else
                        {
                            inst.SourceAnchorClosest = last;
                        }
                        currSource  =  null;
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
                Address   =  ParseHelper.ParseHexWord(arr),
                Raw       =  ParseHelper.ParseHexByteArray(bytes),
                SourceAsm = ll[38..],
                SourceAnchor    = source.FindAnchor(currSource)
            };
        }


        public IEnumerable<RegisterDelta> ParseRegisters(IEnumerable<string> lines)
        {
            foreach (var ll in lines)
            {
                var line   = ll;
                int maxLen = "       rax = 0x00007fff7dd05b18".Length;
                if (line.Length > maxLen)
                {
                    line = line.Substring(0, maxLen);
                }
                if (StringHelper.TrySplitExclusive(line, " = ", out var r))
                {
                    yield return new RegisterDelta()
                    {
                        Register     = r.l.Trim(),
                        ValueRaw  = r.r.Trim(),
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
            var step = new StoryStep();
            
            // FrontMatter
            var c   = 0;
            while (readAllLines[c].StartsWith("#"))
            {
                step.Comment ??= new StoryAnnotation();
                var l = readAllLines[c].Remove(0, 1).Trim();
                if (l.StartsWith("$"))
                {
                    // map
                    if (StringHelper.TrySplitExclusive(l, "=", out var parts))
                    {
                        step.Comment.Tags ??= new List<Tag>();
                        step.Comment.Tags.Add(new Tag()
                        {
                            Name = parts.l.Remove(0,1),
                            Value = parts.r
                        });
                    }
                }
                else// normal line 
                {
                    if (step.Comment.Text == null)
                    {
                        step.Comment.Text ??= "";    
                    }
                    else
                    {
                        step.Comment.Text += "\n";
                    }
                    
                    step.Comment.Text +=  l;
                }
                
                c++;
            }
            
            var l0   = readAllLines[c];

            var clean = l0.Remove(0, 8).TrimEnd(')');
            if (StringHelper.TrySplitExclusive(clean, ", ", out var res))
            {
                step.Delta = ParseRegisters(readAllLines.Skip(c+5)).ToImmutableArray();
                step.Asm   = res.r;
                step.RIP   = ParseHelper.ParseHexWord(res.l);
                return step;
            }
            else
            {
                // cannot read first line, try RIP
                var rip = readAllLines[c + 2]; 
                if (rip.StartsWith("RIP="))
                {
                    step.Delta = ParseRegisters(readAllLines.Skip(c+5)).ToImmutableArray();
                    step.Asm   = null;
                    step.RIP   = ParseHelper.ParseHexWord(rip.Remove(0, 4));
                    return step;
                }

            }
            return null;
        }
        
        
    }

}