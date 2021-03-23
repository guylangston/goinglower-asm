namespace GoingLower.Core.Primitives
{
    public class DBlockProps
    {
        public DBorder Border  { get; set; }
        public DBorder Padding { get; set; }
        public DBorder Margin  { get; set; } 
        
        public DBlockProps()
        {

        }

        public DBlockProps(DBlockProps copy)
        {
            Margin  = new DBorder(copy.Margin);
            Border  = new DBorder(copy.Border);
            Padding = new DBorder(copy.Padding);
        }

        public DBlockProps Set(float margin, float border, float padding)
        {
            this.Margin  = new DBorder(margin);
            this.Border  = new DBorder(border);
            this.Padding = new DBorder(padding);
            return this;
        }
        
    }
}