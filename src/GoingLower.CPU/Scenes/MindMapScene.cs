using System;
using System.Collections.Generic;
using System.Linq;
using GoingLower.Core;
using GoingLower.Core.CMS;
using GoingLower.Core.Drawing;
using GoingLower.Core.Elements;
using GoingLower.Core.Elements.Sections;
using GoingLower.Core.Graph;
using GoingLower.Core.Helpers;
using GoingLower.Core.Primitives;
using GoingLower.CPU.Elements;
using GoingLower.CPU.Model;
using GoingLower.CPU.Parsers;

namespace GoingLower.CPU.Scenes
{

    public class TextSectionX : TextSection<IScene, IContent>
    {
        public TextSectionX(IElement parent, IContent model, DBlock block) : base(parent, model, block)
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
        private TextSectionX body;
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

            

            this.body = Add(new TextSectionX(this, null, new DBlock(150, 300, Block.W, Block.H - 300).Set(10, 2, 20)));            
            UpdateSelected(Model.Nodes.First());

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