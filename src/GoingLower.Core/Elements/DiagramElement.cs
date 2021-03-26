using System;
using GoingLower.Core;
using GoingLower.Core.Docking;
using GoingLower.Core.Drawing;
using GoingLower.Core.Graph;
using GoingLower.Core.Helpers;
using GoingLower.Core.Primitives;
using SkiaSharp;

namespace GoingLower.CPU.Scenes
{
    public enum Decoration
    {
        Square,
        Rect,
        Oval,
        Circle,
        Diamond,
        Triangle
    }

    public class DiagramModel
    {
        public Decoration Decoration { get; set; }
        public string     Id         { get; set; }
        public object     Data       { get; set; }

        public T DataAs<T>() => (T)Data;

        public AProp<string> Title { get; } = new AProp<string>(string.Empty);
        public AProp<string> Text  { get; } = new AProp<string>(string.Empty);
    }

    public interface IElementTheme
    {
        SKPaint GetPaint(IElement element, IAProp prop);
        SKPaint GetPaint(IElement element, string name, object currentValue);
    }
    
    public class DiagramElement : ElementWithModel<DiagramModel> 
    {
        private readonly IElementTheme? theme;
            
        public DiagramElement(IElement parent, DiagramModel model, DBlock? block, IElementTheme? theme) : base(parent, block, model)
        {
            this.theme = theme;
        }

        protected override void Step(TimeSpan step)
        {
            
        }
        
        public bool IsSelected { get; set; }

        protected override void Draw(DrawContext surface)
        {
            surface.Canvas.Save();
            surface.Canvas.ClipRect(Block.Inner.Outset(10, -10).ToSkRect());
            
            
            var ib = Block.Inner;
            surface.Canvas.DrawOval(ib.MM, new SKSize(ib.W/2, ib.H/2) ,  Scene.StyleFactory.GetPaint(this, IsSelected ? "Selected" :  "border"));
            
            surface.DrawText(Model.Title.Value, 
                theme?.GetPaint(this, Model.Title) ?? Scene.StyleFactory.GetPaint(this, "text"),
                Block, BlockAnchor.MM);
            
            surface.DrawText(Model.Text.Value,
                theme?.GetPaint(this, Model.Text) ?? Scene.StyleFactory.GetPaint(this, "SmallFont"), 
                Block, BlockAnchor.BM);
            
            surface.Canvas.Restore();

            if (Model.Data is INode n && n.Edges != null)
            {
                foreach (var edge in n.Edges)
                {
                    if (edge.A == n)
                    {
                        if (Parent.TryFindByModelRecursive<DiagramModel>(
                            x=>object.ReferenceEquals( x.Data, edge.B), out var eleB))
                        {
                            var a = new DockedArrow(
                                new DockPoint(this)
                                {
                                    AnchorInner = true,
                                }, 
                                new DockPoint(eleB)
                                {
                                    AnchorInner = true,
                                    Padding     = new SKPoint(-3,0)
                                }, 
                                GetStyle(IsSelected ? "arrow" : "ArrowGray"))
                            {
                                
                            };
                            a.Step();
                            a.Draw(surface.Canvas);
                        }
                        
                        
                    }
                }
            }
        }
    }
}