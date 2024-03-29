using System;
using System.Linq;
using GoingLower.Core;
using GoingLower.Core.Elements;
using GoingLower.Core.Elements.Sections;
using GoingLower.Core.Helpers;
using GoingLower.Core.Primitives;
using GoingLower.CPU.Animation;
using GoingLower.CPU.Model;
using GoingLower.CPU.Scenes;

namespace GoingLower.CPU.Elements
{
    public class StoryStepPhaseElement : Section<SceneExecute, StoryStep>
    {
        private TextBlockElement text;
        
        public StoryStepPhaseElement(IElement parent, StoryStep model) : base(parent, model, new DBlock(0,0,0,140))
        {
            Title = null;
            Block.Set(0, 0, 0);
            Block.Margin = new DBorder(15, 4, 4, 4);
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
                if (Model.Comment != null)
                {
                    if (Model.Comment.Title != null)  text.WriteLine(Model.Comment.Title, Scene.Styles.FixedFontBlue);
                    if (Model.Comment.Text != null)   text.WriteLine(Model.Comment.Text, Scene.Styles.FixedFontWhite);
                    
                    if (Model.Comment.Tags != null && Model.Comment.Tags.Any())
                    {
                        foreach (var tag in Model.Comment.Tags)
                        {
                            text.WriteLineFormatted($"{tag.Name} <=> {tag.Value}");
                            if (Scene.Cpu.TryGetReg(tag.Name, out var reg))
                            {
                                reg.TagValue  = tag.Value;
                                reg.IsChanged = true;
                            }
                        }
                    }
                }
                
                text.WriteFormatted($"Step: {Scene.Cpu.Story.CurrentIndex}/{Scene.Cpu.Story.Steps.Count}");
                text.WriteLine();

                
            }
            //Block.H = Math.Max(60, text.LastDrawHeight);
        }

    }
}