using System;
using System.Linq;
using Animated.CPU.Animation;

namespace Animated.CPU.Model
{
    public class StoryStepPhaseElement : Section<Scene, StoryStep>
    {
        private TextBlockElement text;
        
        public StoryStepPhaseElement(IElement parent, StoryStep model) : base(parent, model, new DBlock(0,0,0,200))
        {
            Title = null;
            Block.Set(4, 0, 10);
            Block.Margin = new DBorder(30, 4, 4, 4);
        }
        
        protected override void Init()
        {
            text = Add(new TextBlockElement(this, Block, Scene.Styles.FixedFont));
        }
        
        protected override void Step(TimeSpan step)
        {
            text.Clear();
            if (Model != null)
            {
                text.WriteFormatted($"Step: {Scene.Cpu.Story.CurrentIndex}/{Scene.Cpu.Story.Steps.Count}");
                text.WriteLine();

                if (Model.Comment != null)
                {
                    text.WriteLine(Model.Comment.Title);
                    text.WriteLine(Model.Comment.Text);
                    
                    if (Model.Comment.Tags != null && Model.Comment.Tags.Any())
                    {
                        foreach (var tag in Model.Comment.Tags)
                        {
                            text.WriteLineFormatted($"Tag {tag.Name} <- {tag.Value}");

                            if (Scene.Cpu.TryGetReg(tag.Name, out var reg))
                            {
                                reg.TagValue = tag.Value;
                            }
                        }
                    }
                }

                
            }
            //Block.H = Math.Max(60, text.LastDrawHeight);
        }

    }
}