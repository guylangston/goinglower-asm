using System;
using System.Collections.Generic;
using System.Linq;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
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
                segments.Add(item);

                cc += (uint)item.Raw.Length;
            }

            Length = cc;
        }

        public uint Offset { get; }
        public uint Length { get; }

        public List<Segment> segments { get; } = new List<Segment>();

        public class Segment
        {
            public uint   Offset { get; set; }
            public byte[] Raw    { get; set; }
             
            public string Header  { get; set; }
            public string Label   { get; set; }
            public string Source  { get; set; }
            public string Comment { get; set; }

            public override string ToString()
            {
                return $"[{Offset}] {String.Join("", Raw.Select(x=>x.ToString("X")))}, {Label} {Source} {Comment}";
            }
        }
        
        
    }
    
    public class MemoryViewElement : Element<Scene>
    {

        public MemoryViewElement(Scene scene, DBlock b, MemoryView memory) : base(scene, b)
        {
            Memory = memory;
        }

        public MemoryView Memory { get; set; }

        
        public override void Step(TimeSpan step)
        {
            
        }
        
        public override void Draw(SKSurface surface)
        {
            // var stack = new DStack(this.Block, DOrient.Horz);
            // stack.Divide(Memory.segments);
            //
            // var txt = new SKPaint()
            // {
            //     Color = new SKColor(100, 200, 240)
            // };
            //
            // var drawing = new Drawing(surface.Canvas);
            // foreach (var seg in stack.Children)
            // {
            //     drawing.DrawRect(seg);
            //     drawing.DrawText(seg.Domain?.ToString(), txt, seg.Inner.ML);
            // }
        }
    }

}