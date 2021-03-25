using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GoingLower.Core.CMS;
using GoingLower.Core.Graph;
using GoingLower.Core.Helpers;
using GoingLower.CPU.Scenes;

namespace GoingLower.CPU.Model
{
    public class MindMap // : IGraph
    {
        private List<MindMapNode> nodes = new();
        private List<Edge<MindMapNode>> edges = new();
        
        public MindMap()
        {
            // TODO: Could be done with reflection or source generators
            Add(CSharp); 
            Add(Rosyln);     
            Add(IL);         
            Add(JIT1);       
            Add(JIT2);       
            Add(MachineCode);
            Add(MicroCode);
            
            Chain(CSharp, Rosyln, IL, JIT1, MachineCode, MicroCode);
            Link(JIT1, JIT2);
            Link(JIT2, MachineCode);
        }

        public IReadOnlyCollection<MindMapNode> Nodes => nodes;
        
        public MindMapNode CSharp      { get; } = new MindMapNode(nameof(CSharp));
        public MindMapNode Rosyln      { get; } = new MindMapNode(nameof(Rosyln));
        public MindMapNode IL          { get; } = new MindMapNode(nameof(IL));
        public MindMapNode JIT1        { get; } = new MindMapNode(nameof(JIT1));
        public MindMapNode JIT2        { get; } = new MindMapNode(nameof(JIT2));
        public MindMapNode MachineCode { get; } = new MindMapNode(nameof(MachineCode));
        public MindMapNode MicroCode   { get; } = new MindMapNode(nameof(MicroCode));
        
        public MindMapNode Add(MindMapNode node)
        {
            nodes.Add(node);
            return node;
        }
        
        public Edge<MindMapNode> Link(MindMapNode a, MindMapNode b)
        {
            var r = new Edge<MindMapNode>(a, b);
            edges.Add(r);
            ((INodeSetup)a).AddEdge(r);
            ((INodeSetup)b).AddEdge(r);
            return r;
        }

        public void Chain(params MindMapNode[] chain)
        {
            foreach (var (a, b) in GeneralHelper.PairWise(chain))
            {
                Link(a, b);
            }
        }

    }
    
    public class MindMapNode : INode, INodeIdent<string>, INodeSetup
    {
        private List<Edge<MindMapNode>> edges = new();
        

        public MindMapNode(string id)
        {
            Id      = id;
            Content = new LazyContent(this);
        }

        public IEnumerable<IEdge> Edges   => edges;
        public string             Id      { get; }
        public IContent           Content { get; }
        
        
        public DiagramElement<MindMapScene> Display { get; set; }

        // Hidden
        // force graph building to MindMap
        void INodeSetup.AddEdge(IEdge edge)     
        {
            edges.Add((Edge<MindMapNode>)edge);
        }
        object INodeIdent.Id => Id;
        
        public class LazyContent : IContent
        {
            private Lazy<IContent> content;

            public LazyContent(MindMapNode node)
            {
                this.content = new Lazy<IContent>(() => 
                {
                    var f = Path.Combine("/home/guy/repo/cpu.anim/doc/Layers/Code/", $"{node.Id}.md");
                    var lines  = 
                        File.Exists(f) ? File.ReadAllLines(f)
                            :  new string[]
                            {
                                "Missing:",
                                f
                            };
                    
                    var hdr = new ExtendedContentSummary(
                        node.Id,
                        "text/markdown",
                        node.Id,
                        lines.FirstOrDefault(), 
                        new Dictionary<string, object>());
                    
                    ContentBodyTextLines? body = 
                        File.Exists(f) ? new ContentBodyTextLines(File.ReadAllLines(f))
                            :  new ContentBodyTextLines(new string[]
                        {
                            "Missing:",
                            f
                        });
                    
                    

                    return new Content(hdr, body);
                });
            }

            public ExtendedContentSummary Header => content.Value.Header;
            public IContentBody           Body   => content.Value.Body;
        }

        public void Associate(DiagramElement<MindMapScene> display)
        {
            this.Display              = display;
            display.Model.Title.Value = Id;
            display.Model.Text.Value  = Content.Header.Digest;
            display.Model.Data        = this;

        }
    }

    public enum Decoration
    {
        Square,
        Rect,
        Oval,
        Circle,
        Diamond,
        Triangle
    }

}