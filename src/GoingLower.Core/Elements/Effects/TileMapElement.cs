using System;
using System.Collections.Generic;
using System.Linq;
using GoingLower.Core.Drawing;
using GoingLower.Core.Primitives;
using SkiaSharp;

namespace GoingLower.Core.Elements.Effects
{
    public interface ITileMap
    {
        SKSize Size { get; }
        SKPoint this[int x, int y] { get; }
    }

    public struct Tile<T>
    {
        public Tile(T data, SKPoint point, int x, int y)
        {
            Data  = data;
            Point = point;
            X     = x;
            Y     = y;
        }

        public T       Data  { get; }
        public SKPoint Point { get; }
        public int     X     { get;  }
        public int     Y     { get;  }
    }

    public interface ITileMap<T>
    {
        IEnumerable<T> Models { get; }
        IEnumerable<Tile<T>> Tiles { get; }
    }
    
    public abstract  class  TileMapElement<T> : ElementWithModel<ITileMap<T>>, ITileMap
    {
        protected TileMapElement(IElement parent, DBlock? block, ITileMap<T> model) : base(parent, block, model)
        {
        }

        protected abstract void DrawTile(DrawContext surface, Tile<T> map);
        
        
        protected override void Step(TimeSpan step)
        {
            
        }

        protected override void Draw(DrawContext surface)
        {
            foreach (var tile in Model.Tiles)
            {
                DrawTile(surface, tile);
            }
        }
        
        public SKSize Size { get; set; }

        public SKPoint this[int x, int y] => Block.Inner.TL + new SKPoint(Size.Width * x, Size.Height * y);
    }

 
}
