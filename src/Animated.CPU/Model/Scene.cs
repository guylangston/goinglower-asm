using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
{
    

    public class Scene : SceneBase<Cpu>
    {
        public Scene() : base(new StyleFactory())
        {
            Model = ExampleCPU.BuildCPU();
        }

        protected override void InitScene(SKSurface surface)
        {
            Debug.WriteLine("Init");
            Console.WriteLine("Init2");
            
            var size = surface.Canvas.LocalClipBounds;
            var main = new DBlock()
            {
                X = 0,
                Y = 0,
                W = size.Width,
                H = size.Height
            };
            
            //
            // for (int cc = 0; cc < 100; cc++)
            //     Add(new BackGroundNoise(this, main));
            
            
            main.Set(10, 1, 4, new SKColor(100, 0, 100));
            
            var stack = new DStack(main, DOrient.Vert);
            var items = stack.Layout(new IElement[]
            {
                new ElementRegisterFile(this, Model.RegisterFile, null),
                new ALUElement(this, Model.ALU, null),
                new MemoryViewElement(this, null, Model.Instructions),
                new MemoryViewElement(this, null, Model.Stack)
            });

            foreach (var kid in items)
            {
                Add(kid.model);
            }
        }

        public override void StepScene(TimeSpan s)
        {
            Model.Step();
        }

        protected override void DrawOverlay(SKSurface surface)
        {
            var canvas = surface.Canvas;
            canvas.DrawText($"{Steps} frames at {Elapsed.TotalSeconds:0.00} sec", 10, 10, StyleFactory.GetPaint(this, "debug"));
            
            Drawing d = new Drawing(surface.Canvas);


            


            if (TryGetElementFromModel(Model.RIP, out var eRip)
                && TryGetElementFromModel(Model.Instructions.Segments[3], out var eSeg))
            {
                if (eRip is ElementBase eb && !eb.Animator.IsActive)
                {
                    
                    
                    var a = new Arrow()
                    {
                    
                        Start     = eRip.Block.Outer.MR,
                        WayPointA = eRip.Block.Outer.MR + new SKPoint(50, 0),
                    
                        End       = eSeg.Block.Outer.ML,
                        WayPointB = eSeg.Block.Outer.ML + new SKPoint(-50, 0),
                    
                        Style     = StyleFactory.GetPaint(this, "arrow"),
                        LabelText = "Get Next Instruction",
                        ShowHead  = true
                    };
                    a.Draw(surface.Canvas);
                    
                    if (TryGetElementFromModel(Model.ALU.Decode, out var eDecode))
                    {
                        a = new Arrow()
                        {
                            Start = eSeg.Block.Outer.ML,
                     
                            End = eDecode.Block.Outer.MM,
                     
                            Style     = StyleFactory.GetPaint(this, "arrow"),
                            LabelText = "Get Next Instruction",
                            ShowHead  = true
                        };
                        a.Draw(surface.Canvas);    
                    }
                }

                
                
            }
        }
        
        protected override void DrawBackGround(SKSurface surface)
        {
            var canvas = surface.Canvas;
            canvas.Clear(StyleFactory.GetColor(this, "bg"));

           
            
           
           

        }
        
        
        
        public override bool TryGetElementFromModel<T>(T findThis, out IElement found)
        {
            foreach (var element in ChildrenRecursive())
            {
                if (object.ReferenceEquals(element.Model, findThis))
                {
                    found = element;
                    return true;
                }
            }

            found = null;
            return false;
        }





    }


}
