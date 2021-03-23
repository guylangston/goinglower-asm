namespace GoingLower.Core.Primitives
{
    public class DynamicDBlock : DBlock
    {
        public DynamicDBlock()
        {
        }

        public DynamicDBlock(float desiredWidth, float desiredHeight) : base(0,0, desiredWidth, desiredHeight)
        {
            DesiredWidth  = desiredWidth;
            DesiredHeight = desiredHeight;
        }

        public DynamicDBlock(float x, float y, float w, float h, float desiredWidth, float desiredHeight) : base(x, y, w, h)
        {
            DesiredWidth  = desiredWidth;
            DesiredHeight = desiredHeight;
        }

        public float DesiredWidth  { get; set; }
        public float DesiredHeight { get; set; }
    }
}