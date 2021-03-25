using System;
using System.Collections.Generic;
using System.Linq;

namespace GoingLower.Core.Graph
{
    public interface ITreeNode
    {
        ITreeNode?             Parent   { get; }
        IEnumerable<ITreeNode> Children { get; }
    }

    public interface INodeIdent
    {
        object Id { get; }
    }
    
    public interface INodeIdent<TIdent> : INodeIdent
    {
        new TIdent Id { get; }
    }

    public interface INodeValue 
    {
        object Value { get; set; }
    }
    
    public interface INodeValue<TVal> : INodeValue
    {
        new TVal Value { get; set; }
    }

    public interface INode
    {
        IEnumerable<IEdge> Edges { get; }
    }
    
    
    public interface INodeSetup
    {
        void AddEdge(IEdge edge);
    }
    

    public interface IGraph
    {
        IReadOnlyCollection<INode> Nodes { get; }
        TNode Add<TNode>(TNode node) where TNode:INode;
        TEdge AddEdge<TEdge>(TEdge edge) where TEdge:IEdge;
    }

    // Edges should not be set by Node, rather the graph sets the edges and nodes
    public interface IEdge
    {
        INode         A         { get; }
        INode         B         { get; }
    }

    public class Edge<T> : IEdge where T:INode
    {
        public Edge(T a, T b)
        {
            A         = a;
            B         = b;
        }
        
        public T         A         { get; }
        public T         B         { get; }
        
        
        INode IEdge.A => (INode)A;
        INode IEdge.B => (INode)B;
    }

    public static class TreeHelper
    {
        public static int Depth(ITreeNode node) => throw new NotImplementedException();
    }


    public static class GraphHelper
    {
        // https://en.wikipedia.org/wiki/Rank_(graph_theory)
        public static int GetLongestPath(INode node) => WalkGraph(node).Count;


        public static List<INode> WalkGraph(INode node)
        {
            var visted = new List<INode>();
            Visit(node, visted);
            return visted;
        }

        static void Visit(INode node, List<INode> visited)
        {
            if (node?.Edges is null || !node.Edges.Any()) return;
            
            visited.Add(node);
            foreach (var edge in node.Edges)
            {
                if (edge.A == node)  // outward
                {
                    if (!visited.Contains(edge.B))
                    {
                        Visit(edge.B, visited);
                    }
                }
            }
        } 
    }
    
    
}