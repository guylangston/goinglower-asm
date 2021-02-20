using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
{



    public class RegisterElement : Element<Scene>
    {

        
        public RegisterElement(Scene scene, DBlock b) : base(scene, b)
        {
        
        }
        
        public override void Step(TimeSpan step)
        {
            
        }
        
        public override void Draw(SKSurface surface)
        {
            var canvas = surface.Canvas;
            var draw   = new Drawing(canvas);
            
            // map to scheme
            int cc = 0;


            var itemPaint = new DBlockProps().Set(2, 2, 2, new SKColor(150, 200, 250));
            
            
            var stack       = new DStack(this.Block, DOrient.Horz, itemPaint);
            
            stack.Divide(Scene.cpu.RegisterFile);
            
            foreach (var panel in stack.Children)
            {
                draw.DrawRect(panel);
                var reg = panel.Domain as Register;
                
                draw.DrawText($"[{reg.Id}]", Scene.t1, panel.Inner.TL);
                draw.DrawTextRight(reg.Name ?? "", Scene.t1a, panel.Inner.TR + new SKPoint(-5, 0));
                draw.DrawTextRight(reg.Value.ToString("X"), Scene.t1, panel.Inner.BR + new SKPoint(-5, -15));
                
                // new Arrow()
                // {
                //     Style     = Scene.t2,
                //     Start     = panel.Inner.MR,
                //     End       = (panel.Inner.MR + new SKPoint(35, 5)),
                //     LabelText = reg.Id,
                //     ShowHead  = true
                // }.Draw(canvas);
                    
                cc++;
            }
        }
    }

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
            Offset   = 0;
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
            public uint   Offset  { get; set; }
            public byte[] Raw     { get; set; }
             
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
            var stack = new DStack(this.Block, DOrient.Horz, new DBlockProps().Set(0, 1, 5, new SKColor(0xeeeee)));
            stack.Divide(Memory.segments);
            
            var txt = new SKPaint()
            {
                Color       = new SKColor(100, 200, 240)
            };

            var drawing = new Drawing(surface.Canvas);
            foreach (var seg in stack.Children)
            {
                drawing.DrawRect(seg);
                drawing.DrawText(seg.Domain?.ToString(), txt, seg.Inner.ML);
            }
        }
    }

}