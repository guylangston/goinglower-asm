using System;
using System.Collections.Generic;
using System.Linq;
using GoingLower.Core;
using GoingLower.Core.CMS;
using GoingLower.Core.Drawing;
using GoingLower.Core.Elements;
using GoingLower.Core.Elements.Effects;
using GoingLower.Core.Graph;
using GoingLower.Core.Helpers;
using GoingLower.Core.Primitives;
using GoingLower.CPU.Elements;
using GoingLower.CPU.Model;
using GoingLower.CPU.Parsers;
using SkiaSharp;

namespace GoingLower.CPU.Scenes
{

    public class TextContentSection : TextSection<IScene, IContent>
    {
        public TextContentSection(IElement parent, IContent model, DBlock block) : base(parent, model, block)
        {
            Parser = new SourceParser(new SyntaxMarkDown());
        }

        protected override IReadOnlyList<string> GetLines(IContent model)
        {
            return model.Body.ReadLines();
        }
    }
   

    public class MindMapScene : SceneBase<MindMap, StyleFactory>
    {
        private NetworkElement network;
        private TextContentSection body;
        private MindMapNode? selected;

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
            network = Add(new NetworkElement(this, headerBlock, GetNode)
            {
                // Apply Fixups to the default layout
                AfterLayout = (e, n, rank, stack) => {
                    if (n is MindMapNode nn)
                    {
                        if (nn.Id == "JIT2")
                        {
                            e.Block.Y -= 40;
                        }
                        nn.Rank  = rank;
                        nn.Stack = stack;
                    }
                }
            });
            foreach (var node in Model.Nodes)
            {
                node.Associate(network.Add(new DiagramElement(network, new DiagramModel()
                {
                    Decoration = Decoration.Oval,
                    Id = node.Id
                }, DBlock.JustWidth(200).Set(10, 2, 10), themeHeader)
                ));
            }
            network.Layout();

            

            this.body = Add(new TextContentSection(this, null, new DBlock(150, 300, Block.W, Block.H - 300).Set(10, 2, 20))
            {
                ShowLineNumbers = false
            });            
            UpdateSelected(Model.Nodes.First());


            var z = SKBitmap.Decode("/home/guy/Pictures/sample1.png");

            var img  = new SKBitmap(250, 100);
            var imgc = new SKCanvas(img);
            imgc.DrawBitmap(z, 1, 1);
            var munro = new SKPaint()
            {
                Typeface =SKTypeface.FromFamilyName(
                    "Munro", 
                    SKFontStyleWeight.Normal, 
                    SKFontStyleWidth.Normal, 
                    SKFontStyleSlant.Upright),
                TextSize = 10,
                Color = SKColors.Aquamarine
            };
            SKRect munroSize = new SKRect();
            munro.MeasureText("|", ref munroSize);
            imgc.DrawText("Hello World! 0123456789", 10, 20, munro);
            imgc.DrawText("The lazy brown cow jumped over the moon", 10, 20 +munroSize.Height, munro);
            
            
            var on = new SKPaint()
            {
                StrokeWidth = 1,
                Color       = SKColors.Yellow,
                Style       = SKPaintStyle.StrokeAndFill
            };
            
            // https://www.fontspace.com/munro-font-f14903
            var r     = new Random();
            var model = new VirtualScreen(new SKSize(3,3), new SKSize(1, 1),  img.Width, img.Height);
            
            var play2 = Add(new VirtualScreenElement(this, new DBlock(1000, 1000, 500, 500), model));
            play2.Model.Fill( ((x,y) => {

                SKPaint f       = null;
                var     clr = img.GetPixel(x, y);
                if (clr != SKColor.Empty)
                {
                    f = new SKPaint()
                    {
                        Color = clr,
                        Style = SKPaintStyle.Fill
                    };
                }
                return new VirtualScreenPixel()
                {
                    FG = f
                };
            }));
            


        }

        private void UpdateSelected(MindMapNode newNode)
        {
            if (selected != null)
            {
                selected.Display.IsSelected = false;    
            }
            
            newNode.Display.IsSelected = true;
            selected                   = newNode;
            body.Model                 = selected.Content;
            body.IsSourceChanged       = true;
        
        }

        private INode? GetNode(IElement arg)
        {
            if (arg is DiagramElement dm && dm.Model.Data is INode node) return node;
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
            if (key == Keys.Right)
            {
                
                if (network.TryFind<DiagramElement, DiagramModel>((e, m) => e.IsSelected, out var ee, out var mm))
                {
                    var current = mm.DataAs<MindMapNode>();
                    var next = network.Map.AllB.Cast<MindMapNode>().FirstOrDefault(x => x.Rank == current.Rank - 1);
                    if (next != null)
                    {
                        UpdateSelected(next);

                    }
                }
            }
            else if (key == Keys.Left)
            {
                
                if (network.TryFind<DiagramElement, DiagramModel>((e, m) => e.IsSelected, out var ee, out var mm))
                {
                    var current = mm.DataAs<MindMapNode>();
                    var next    = network.Map.AllB.Cast<MindMapNode>().FirstOrDefault(x => x.Rank == current.Rank + 1);
                    if (next != null)
                    {
                        UpdateSelected(next);
                    }
                }
            }
        }

        public override void MousePress(uint eventButton, double eventX, double eventY, object interop)
        {
         
        }
    }
}