using System;
using System.Collections.Generic;
using Animated.CPU.Animation;

namespace Animated.CPU.Model
{
    public class Dialog
    {
        public string       Title { get; set; }
        public List<string> Lines { get; } = new List<string>();
    }
    
    public class DialogElement : Section<Scene, Dialog>
    {
        private TextBlockElement text;
        
        public DialogElement(IElement parent, Dialog model, DBlock block) : base(parent, model, block)
        {
            IsHidden = true;
            Title    = Model.Title;
        }

        protected override void Init()
        {
            this.text = this.Add(new TextBlockElement(this, Block, Scene.Styles.FixedFontGray));
            
            Block.Z      = 100;
            text.Block.Z = 100;
        }

        protected override void Step(TimeSpan step)
        {
            Title = Model.Title;
            this.text.Clear();
            foreach (var line in Model.Lines)
            {
                text.WriteLine(line);
            }
        }
    }
}