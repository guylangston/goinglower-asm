using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model.ModelElements
{
    public class MemoryViewElement : Section<Scene, MemoryView>
    {
        public MemoryViewElement(IElement scene, DBlock b, MemoryView memory) : base(scene, memory, b)
        {
            
        }

        private StackElement stack;
        protected override void Init()
        {
            stack = Add(new StackElement(this, Block, DOrient.Vert));

            for (int i = 0; i < 20; i++)
            {
                stack.Add(new SegmentElement(stack, null, new DynamicDBlock()
                {
                    DesiredHeight = 30
                }));    
            }
            
            
            
            
            
        }

        protected override void Step(TimeSpan step)
        {
            foreach (var seg in ChildrenAre<SegmentElement>())
            {
                seg.IsEnabled = false;
            }
            
            var curr   = Model.GetByAddress(Scene.Cpu.RIP.Value);
            var offset = 0;
            if (curr != null)
            {
                var idx = Model.Segments.IndexOf(curr);
                
                if (idx > 15)  offset = idx - LookBack;
            }

            foreach (var pair in Enumerable.Zip(
                stack.ChildrenAre<SegmentElement>(),
                Model.Segments.Skip(offset).Take(stack.ChildrenCount)
                ))
            {
                if (pair.First != null && pair.Second != null)
                {
                    pair.First.Model     = pair.Second;
                    pair.First.IsEnabled = true;
                }
                else if (pair.First != null)
                {
                    pair.First.IsEnabled = false;
                }
                
            }
            
            

        }

        public int Max      { get; set; } = 16;
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
            
          
        }

        protected override void Step(TimeSpan step)
        {
            txt.Clear();
            if (Model != null)
            {
                IsEnabled = true;
                if (Model.SourceAnchor != null)
                {
                    txt.WriteLine(Model.SourceAnchor, Scene.Styles.FixedFontYellow);
                }

                txt.Write("|   ", Scene.Styles.FixedFontDarkGray);
                txt.WriteLine(Model.SourceAsm, Scene.Styles.FixedFontCyan);
            
                txt.Write("|   ", Scene.Styles.FixedFontDarkGray);
                txt.Write($"{Model.Raw?.ToHex()} @ [{DisplayHelper.ToHex(Model.Address)}]".PadLeft(40), Scene.Styles.FixedFontDarkGray);
            
            
                txt.Block.H = Block.H = (Model.SourceAnchor != null ? txt.LineHeight*4 : txt.LineHeight*3) + 10;
            
                txt.Background = this.IndexInParent % 2 == 0
                    ? Scene.Styles.BackGround
                    : Scene.Styles.BackGroundAlt;
            

                txt.IsEnabled = Block.Y <= Parent.Block.Inner.Y2;    
            }
            else
            {
                IsEnabled = false;
            }
            
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


                        // var r = new SKRect(
                        //     Block.X,
                        //     line.Line.Y,
                        //     Block.X2,
                        //     line.Line.Y + line.Line.H);
                        //
                        // surface.Canvas.DrawRect(r, stylesArrow);
                    }
                    
                }
                    
                
            }
        }

        protected override void Draw(DrawContext surface)
        {


        }
    }

}