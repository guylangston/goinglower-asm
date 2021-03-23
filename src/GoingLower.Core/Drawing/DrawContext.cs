using SkiaSharp;

namespace GoingLower.Core.Drawing
{
    public class DrawContext : DrawContextBase // Use Base Class to add draw func
    {
        public IScene Scene { get; }

        public DrawContext(IScene scene, SKCanvas canvas) : base(canvas)
        {
            Scene = scene;
        }

        public void DrawHighlight(IElement e)
        {
            DrawHighlight(e.Block.BorderRect.ToSkRect(), e.Scene.StyleFactory.GetPaint(e, "Highlighted"), e.Block.Border.All);
        }
    }
}