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
    
    public class MemoryViewElement : Section<Scene, MemoryView>
    {

        public MemoryViewElement(IElement scene, DBlock b, MemoryView memory) : base(scene, memory, b)
        {
            
        }
       
        
       

        protected override void Step(TimeSpan step)
        {
            ClearChildren();
            
            var stack = Add(new StackElement(this, Block, DOrient.Vert));
            

            var curr   = Model.GetByAddress(Scene.Cpu.RIP.Value);
            var offset = 0;
            if (curr != null)
            {
                var idx                    = Model.Segments.IndexOf(curr);
                if (idx > LookBack) offset = idx - LookBack;
            }
            
            foreach (var seg in Model.Segments.Skip(offset).Take(Max))
            {
                stack.Add(new SegmentElement(stack, seg, null), 30);
            }
        }

        public int Max { get; set; } = 16;
        public int LookBack { get; set; } = 3;
    }

    public class SegmentElement : Element<Scene, MemoryView.Segment>
    {
        private ByteArrayElement mem;
        private TextBlockElement txt;

        public SegmentElement(IElement parent, MemoryView.Segment model, DBlock block) : base(parent, model, block)
        {
            
        }

        public bool IsSelected => Model.ContainsAddress(Scene.ElementALU.Fetch.Model.RIP);

        protected override void Init()
        {
            txt = Add(new TextBlockElement(this, Block, Scene.Styles.FixedFont));
            
            if (Model.SourceAnchor != null)
            {
                txt.WriteLine(Model.SourceAnchor, Scene.Styles.FixedFontYellow);
            }

            txt.Write("|   ", Scene.Styles.FixedFontDarkGray);
            txt.WriteLine(Model.SourceAsm, Scene.Styles.FixedFontCyan);
            
            txt.Write("|   ", Scene.Styles.FixedFontDarkGray);
            txt.Write($"{Model.Raw?.ToHex()} @ [{DisplayHelper.ToHex(Model.Address)}]".PadLeft(40), Scene.Styles.FixedFontDarkGray);
        }

        protected override void Step(TimeSpan step)
        {
            txt.Block.H = Block.H = (Model.SourceAnchor != null ? txt.LineHeight*4 : txt.LineHeight*3) + 10;
            
            txt.Background = this.IndexInParent % 2 == 0
                ? Scene.Styles.BackGround
                : Scene.Styles.BackGroundAlt;
            

            txt.IsEnabled = Block.Y <= Parent.Block.Inner.Y2;
        }

        protected override void Decorate(DrawContext surface)
        {
            if (IsSelected)
            {
                surface.DrawHighlight(Block.Outer.ToSkRect(), Scene.Styles.Selected, 1f);

                var l = Model.SourceAnchor ?? Model.SourceAnchorClosest;
                if (l != null)
                {
                    var line = Scene.ElementCode.GetLine(l.Line);
                    if (line != null)
                    {
                        var stylesArrow = Scene.Styles.Arrow;
                        new Arrow()
                        {
                            Start     = Block.Inner.MR,
                            WayPointA = Block.Inner.MR + new SKPoint(20, 0),
                            WayPointB = line.LastDraw + new SKPoint(-20, 2),
                            End       = line.LastDraw + new SKPoint(Block.W, 2),
                            Style     = stylesArrow,
                        }.Draw(surface.Canvas);
                        var r = line.LastDrawRect;
                        r.Inflate(2, -2);
                        surface.Canvas.DrawRect(r, stylesArrow);
                    }
                    
                }
                    
                
            }
        }

        protected override void Draw(DrawContext surface)
        {


        }
    }

}