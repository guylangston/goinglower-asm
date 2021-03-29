using System;
using System.Collections.Generic;
using GoingLower.Core.Drawing;
using GoingLower.Core.Primitives;
using SkiaSharp;

namespace GoingLower.Core.Elements.Effects
{
    public class VirtualScreen : ITileMap<VirtualScreenPixel>
    {
        public VirtualScreen(SKSize pixelSize, SKSize pixelSpace,  int width, int height)
        {
            PixelSize  = pixelSize;
            PixelSpace = pixelSpace;
            TileSize   = pixelSize + pixelSpace + pixelSpace;
            Width      = width;
            Height     = height;

            pixels = new VirtualScreenPixel[width, height];
        }

        public SKSize PixelSize  { get; }
        public SKSize PixelSpace { get; }
        public SKSize TileSize   { get; }

        public int Width  { get;  }
        public int Height { get;  }

        public VirtualScreenPixel[,] pixels;

        public void Fill(Func<int, int, VirtualScreenPixel> cell)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    pixels[x, y]   = cell(x, y);
                    pixels[x, y].X = x;
                    pixels[x, y].Y = y;
                }
            }
        }
        
        public IEnumerable<VirtualScreenPixel> Models
        {
            get
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        yield return pixels[x, y];
                    }
                }
            }
        }
        
        public IEnumerable<Tile<VirtualScreenPixel>> Tiles
        {
            get
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        yield return new Tile<VirtualScreenPixel>(
                            pixels[x, y],
                            new SKPoint(x * TileSize.Width, y * TileSize.Height),
                            x, y);
                    }
                }
            }
        }
    }
    
    public class VirtualScreenPixel
    {
        public int     X  { get; set; }
        public int     Y  { get; set; }
        public SKPaint FG { get; set; }
    }

    public class VirtualScreenElement : TileMapElement<VirtualScreenPixel>
    {
        public VirtualScreenElement(IElement parent, DBlock? block, VirtualScreen model) : base(parent, block, model)
        {
        }

        public new VirtualScreen Model => (VirtualScreen)base.Model;

        protected override void DrawTile(DrawContext surface, Tile<VirtualScreenPixel> map)
        {
            if (map.Data?.FG == null) return;


            if (true)
            {
                var p = Block.Inner.TL + map.Point;
                var r = new SKRect(p.X + Model.PixelSpace.Width, p.Y + Model.PixelSpace.Height,
                    p.X + Model.PixelSpace.Width + Model.PixelSize.Width, p.Y + Model.PixelSpace.Height + Model.PixelSize.Height);
                surface.Canvas.DrawRect(r, map.Data.FG);
                
            }
            else
            {
                var ht = new SKPoint(Model.TileSize.Width / 2, Model.TileSize.Height / 2);
                var p  = Block.Inner.TL + map.Point + ht;
                
                surface.Canvas.DrawCircle(p.X, p.Y, Model.PixelSize.Width/2, map.Data.FG);
            }


        }
        
    }
}