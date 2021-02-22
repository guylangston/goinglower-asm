using System;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
{
    public class ByteArrayModel
    {
        public ByteArrayModel(byte[] bytes, string comment, string parsedValue)
        {
            Bytes       = bytes;
            Comment     = comment;
            ParsedValue = parsedValue;
        }
        public byte[] Bytes   { get; set; }
        public string Comment { get; set; }
        public string ParsedValue { get; set; }
    }
    
    
    public class ByteArrayElement : Element<Scene, ByteArrayModel>
    {

        public ByteArrayElement(Scene scene, ByteArrayModel model) : base(scene, model)
        {
        }
        public ByteArrayElement(IElement parent, ByteArrayModel model) : base(parent, model)
        {
        }
        public ByteArrayElement(Scene scene, ByteArrayModel model, DBlock block) : base(scene, model, block)
        {
        }
        public ByteArrayElement(IElement parent, ByteArrayModel model, DBlock block) : base(parent, model, block)
        {
        }


        public override void Step(TimeSpan step)
        {
            
        }
        public override void Draw(SKSurface surface)
        {
            var x = Block.Inner.X;
            var y = Block.Inner.Y;

            var p = Scene.StyleFactory.GetPaint(this, "hex");
            var hint = Scene.StyleFactory.GetPaint(this, "text");
            
            var cell = Scene.StyleFactory.GetPaint(this, "border");
            
            for (int cc = 0; cc < Model.Bytes.Length; cc++)
            {
                var    txt    = Model.Bytes[cc].ToString("X").PadLeft(2, '0');
                SKRect bounds = new SKRect();
                p.MeasureText(txt, ref bounds);

                var tx = x ;
                var ty = y - bounds.Top;
                surface.Canvas.DrawText(txt, tx, ty, p);

                var textRegion = new SKRect(x, y, x + bounds.Width, y + bounds.Height);
                
                var s      = 2.5f;
                var border = new SKRect(textRegion.Left - s, textRegion.Top - s, textRegion.Right + s + s, textRegion.Bottom + s + s);
                surface.Canvas.DrawRect(border, cell);

                x = border.Right + 1;
            }
            
            if (!string.IsNullOrWhiteSpace(Model.Comment) || !string.IsNullOrWhiteSpace(Model.ParsedValue))
            {   
                x += 5;
                var    text     = $"-> '{Model.ParsedValue}' ; {Model.Comment} ";
                SKRect textSize = new SKRect();
                hint.MeasureText(text, ref textSize);
                surface.Canvas.DrawText(text, x, y - textSize.Top, hint);
                
            }
            


        }
    }
}