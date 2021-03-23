using System;
using System.Collections.Generic;
using System.Linq;
using GoingLower.CPU.Animation;
using SkiaSharp;

namespace GoingLower.CPU.Model
{
  
    
    public enum MemoryHint
    {
        Decimal,
        Hex,
        String
    }
    
    public class MemoryView
    {
        public MemoryView(IEnumerable<Segment> setup)
        {
            Offset = 0;
            uint cc = 0;
            
            foreach (var item in setup)
            {
                item.Offset = cc;
                Segments.Add(item);

                cc += (uint)item.Raw.Length;
            }

            Length = cc;
        }

        public uint Offset { get; }
        public uint Length { get; }

        public List<Segment> Segments { get; } = new List<Segment>();
        

        public class Segment
        {
            public uint              Offset       { get; set; }
            public byte[]            Raw          { get; set; }
            public string            Header       { get; set; }
            public string            Label        { get; set; }
            public string            SourceAsm    { get; set; }
            public string            Comment      { get; set; }
            public ulong             Address      { get; set; }
            public SourceFileAnchor? SourceAnchor { get; set; }
            public SourceFileAnchor? SourceAnchorClosest { get; set; }

            public override string ToString() => 
                $"[{Address:X}|{Offset}] {RawAsString(),16}, {Label} {SourceAsm} {Comment} // {SourceAnchor}";

            public string RawAsString() => String.Join("", Raw.Select(x => x.ToString("X")));

            public bool ContainsAddress(ulong addr) 
                => Address <= addr && addr < (Address + (ulong)Raw.Length);
        }


        public Segment? GetByAddress(ulong addr) 
            => Segments.FirstOrDefault(x => x.Address <= addr && addr < x.Address + (ulong)x.Raw.Length);
    }

    
}