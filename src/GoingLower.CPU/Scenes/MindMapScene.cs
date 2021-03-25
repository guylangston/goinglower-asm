using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GoingLower.Core;
using GoingLower.Core.CMS;
using GoingLower.Core.Docking;
using GoingLower.Core.Drawing;
using GoingLower.Core.Elements;
using GoingLower.Core.Graph;
using GoingLower.Core.Helpers;
using GoingLower.Core.Primitives;
using GoingLower.CPU.Model;
using SkiaSharp;

namespace GoingLower.CPU.Scenes
{
    
    public class DiagramModel
    {
        public Decoration Decoration { get; set; }
        public string     Id         { get; set; }
        public object     Data       { get; set; }

        public AProp<string> Title { get; } = new AProp<string>(string.Empty);
        public AProp<string> Text  { get; } = new AProp<string>(string.Empty);
    }

    public interface IElementTheme
    {
        SKPaint GetPaint(IElement element, IAProp prop);
        SKPaint GetPaint(IElement element, string name, object currentValue);
    }

    public class DiagramElement<TScene> : Element<TScene, DiagramModel>  where TScene:IScene
    {
        private readonly IElementTheme? theme;
            
        public DiagramElement(IElement parent, DiagramModel model, DBlock? block, IElementTheme? theme) : base(parent, model, block)
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
                                    AnchorInner = true
                                }, 
                                new DockPoint(eleB)
                                {
                                    AnchorInner = true
                                }, 
                                GetStyle(IsSelected ? "arrow" : "ArrowGray"))
                            {
                                ArrowPop = 2
                            };
                            a.Step();
                            a.Draw(surface.Canvas);
                        }
                        
                        
                    }
                }
            }
        }
    }
    
    
    
    public class MindMapScene : SceneBase<MindMap, StyleFactory>
    {
        

        public MindMapScene(string name, StyleFactory styleFactory, DBlock block) : base(name, styleFactory, block)
        {
            Model = new MindMap();
        }

        protected override void DrawOverlay(DrawContext drawing)
        {
         
        }

        protected override void DrawBackGround(DrawContext drawing)
        {
            
        }

        protected override void InitScene()
        {
            IElementTheme themeHeader = null; 
           
            var headerBlock = new DBlock(0, 50, Block.W, 200).Inset(20, 20);
            var network     = Add(new NetworkElement(this, headerBlock, GetNode));
            foreach (var node in Model.Nodes)
            {
                node.Associate(network.Add(new DiagramElement<MindMapScene>(network, new DiagramModel()
                {
                    Decoration = Decoration.Oval,
                    Id = node.Id
                }, DBlock.JustWidth(200).Set(10, 2, 10), themeHeader)
                ));
            }
            network.Layout();

            Model.Nodes.First().Display.IsSelected = true;
        }

        private INode? GetNode(IElement arg)
        {
            if (arg is DiagramElement<MindMapScene> dm && dm.Model.Data is INode node) return node;
            return null;
        }

        protected override void InitSceneComplete()
        {
         
        }

        public override void ProcessEvent(string name, object args, object platform)
        {
         
        }

        public override void KeyPress(string key, object platformKeyObject)
        {
         
        }

        public override void MousePress(uint eventButton, double eventX, double eventY, object interop)
        {
         
        }
    }
}