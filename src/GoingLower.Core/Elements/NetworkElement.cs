using System;
using System.Collections.Generic;
using System.Linq;
using GoingLower.Core.Collections;
using GoingLower.Core.Drawing;
using GoingLower.Core.Graph;
using GoingLower.Core.Helpers;
using GoingLower.Core.Primitives;

namespace GoingLower.Core.Elements
{
    public class NetworkElement : ElementBase
    {
        

        public NetworkElement(IElement parent, DBlock? block, Func<IElement, INode?> getNode) : base(parent, block)
        {
            this.getNode = getNode;
        }

        protected override void Init()
        {
            base.Init();
        }

        private Func<IElement, INode?> getNode;

        public void Layout()
        {
            BiMap<IElement, INode> map = new();
            foreach (var kid in Children)
            {
                var node = getNode(kid);
                if (node != null)
                {
                    map.Add(kid, node);        
                }
            }
            
            var partition = PartitionByRank(map.AllB);

            var x = Block.X;
            for (int r = partition.MaxRank-1; r > 0; r--)
            {
                var inRank = partition[r];
                if (inRank.Any())
                {
                    var y = Block.Y;
                    foreach (var node in inRank)
                    {
                        var e = map[node];
                        e.Block.X =  x;
                        e.Block.Y =  y;
                        y         += 200;
                    }
                }
                x += 200;
            }
        }

        protected override void Step(TimeSpan step)
        {
            

        }

        private RankedCollection<INode> PartitionByRank(IEnumerable<INode> nodes)
        {
            var res = new RankedCollection<INode>(x=>GraphHelper.WalkGraph(x).Count);
            foreach (var node in nodes)
            {
                res.Add(node);
            }
            return res;
        }

        protected override void Draw(DrawContext surface)
        {
            // Draw arrows

        }

      
    }
}