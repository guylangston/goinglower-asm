using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model.ModelElements
{
    public class MemoryViewElement : Section<SceneExecute, MemoryView>
    {
        private StackElement stack;

        public MemoryViewElement(IElement scene, DBlock b, MemoryView memory) : base(scene, memory, b)
        {
            
        }

        public int Max      { get; set; } = 16;
        public int LookBack { get; set; } = 3;
        
        protected override void Init()
        {
            stack = Add(new StackElement(this, Block, DOrient.Vert));

            for (int i = 0; i < 20; i++)
            {
                stack.Add(new SegmentElement(stack, null, null));    
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


    }
    
    public class SegmentElement : Element<SceneExecute, MemoryView.Segment>
    {
        
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
             
                txt.Write($"@{DisplayHelper.ToHex(Model.Address)} ", Scene.Styles.FixedFontDarkGray);
                txt.Write(Model.Raw?.ToHex().PadRight(8), Scene.Styles.FixedFontCyan);
                txt.Write(" ", Scene.Styles.FixedFontDarkGray);
                txt.WriteLine(Model.SourceAsm, Scene.Styles.FixedFontYellow);

                
                Block.H = txt.Block.H = txt.CalcHeight();
            
                // txt.Background = this.IndexInParent % 2 == 0
                //     ? Scene.Styles.BackGround
                //     : Scene.Styles.BackGroundAlt;
            

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
                surface.DrawHighlight(this);

                var l = Model.SourceAnchor ?? Model.SourceAnchorClosest;
                if (l != null)
                {
                    var line = Scene.ElementCode.GetLine(l.Line);
                    if (line != null)
                    {
                        var arr = new DockedArrow(
                            new DockPoint(this),
                            new DockPoint(Scene.ElementCode, line)
                            {
                                AnchorInner = true
                            },
                            Scene.Styles.Arrow
                        );
                        arr.Step();
                        arr.Draw(surface.Canvas);
                        
                        
                        // Custom: outline line
                        surface.DrawRect(
                            new DBlock(
                                line.LastDrawRect.Left, 
                                line.LastDrawRect.Top + 1, 
                                Scene.ElementCode.Block.Inner.W, 
                                line.LastDrawRect.Height + 3),
                            Scene.Styles.Arrow);
                    }
                    
                }
                    
                
            }
        }

        protected override void Draw(DrawContext surface)
        {


        }
    }

}