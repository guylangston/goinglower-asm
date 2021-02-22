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
            public uint   Offset { get; set; }
            public byte[] Raw    { get; set; }
             
            public string Header  { get; set; }
            public string Label   { get; set; }
            public string Source  { get; set; }
            public string Comment { get; set; }

            public override string ToString() => $"[{Offset}] {RawAsString()}, {Label} {Source} {Comment}";

            public string RawAsString() => String.Join("", Raw.Select(x => x.ToString("X")));
            
        }
        
        
    }
    
    public class MemoryViewElement : Element<Scene, MemoryView>
    {

        public MemoryViewElement(Scene scene, DBlock b, MemoryView memory) : base(scene, memory, b)
        {
            
        }

        
        
        public override void Init(SKSurface surface)
        {
            var stack = new DStack(this.Block, DOrient.Horz);

            foreach (var item in stack.Layout(Model.Segments))
            {
                item.block.Set(0, 1, 2, null);
                var e = Add(new SegmentElement(this, item.model, item.block)
                {
                    
                });
            }
        }

        
        public override void Step(TimeSpan step)
        {
            
        }
        
        public override void Draw(SKSurface surface)
        {
            
        }
    }

    public class SegmentElement : Element<Scene, MemoryView.Segment>
    {

        public SegmentElement(Scene scene, MemoryView.Segment model) : base(scene, model)
        {
        }
        public SegmentElement(IElement parent, MemoryView.Segment model) : base(parent, model)
        {
        }
        public SegmentElement(Scene scene, MemoryView.Segment model, DBlock block) : base(scene, model, block)
        {
        }
        public SegmentElement(IElement parent, MemoryView.Segment model, DBlock block) : base(parent, model, block)
        {
        }
        public override void Step(TimeSpan step)
        {
            
        }
        public override void Draw(SKSurface surface)
        {
            var canvas = surface.Canvas;
            var draw   = new Drawing(canvas);
            
            var sBorder = Scene.StyleFactory.GetPaint(this, "border");
            var sText   = Scene.StyleFactory.GetPaint(this, "text");
            draw.DrawRect(Block, sBorder);
            
            //draw.DrawTextCenter(Model?.ToString(), sText, Block.Inner.MM);
            draw.DrawText($"+{Model.Offset,-10} -> {Model.RawAsString()}", sText, Block, BlockAnchor.TL);
            draw.DrawText(""+Model.Source, sText, Block, BlockAnchor.ML, new SKPoint(70, 0));
            
            draw.DrawText(""+Model.Comment, sText, Block, BlockAnchor.MR);
            
            draw.DrawText(""+Model.Label, sText, Block, BlockAnchor.BL);
        }
    }

}