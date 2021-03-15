using System;
using System.Collections.Generic;
using System.Linq;
using Animated.CPU.Animation;
using Animated.CPU.Parsers;

namespace Animated.CPU.Model
{
    public class SceneLayers : SceneBase<Cpu, StyleFactory>
    {
        public SceneLayers(StyleFactory styleFactory, DBlock b) : base(styleFactory, b)
        {
        }
        
        public bool                   UseEmbelishments { get; set; } = true;
        private SourceCodeSection current;
        private List<SourceCodeSection> sections;

        protected override void InitScene()
        {
            if (UseEmbelishments)
            {
                for (int cc = 0; cc < 100; cc++)
                    Add(new BackGroundNoise(this, Block));
            }
            
            var stack = Add(new StackElement(this, Block, DOrient.Horz, StackMode.OverrideSize));

            var items = new SourceFile?[]
            {
                Model.Story.MainFile,
                Model.Story.IL , 
                Model.Story.Asm , 
                Model.Story.Binary , 
            };
            sections = new List<SourceCodeSection>();
            foreach (var file in items.Where(x=>x!= null))
            {
                var code = new SourceCodeSection(stack, file, new DBlock().Set(5, 1, 5));
                if (file.Name.EndsWith(".cs"))
                {
                    code.Parser = new SourceParser(new SyntaxCSharp());
                }
                else if (file.Name.EndsWith(".asm"))
                {
                    code.Parser = new SourceParser(new SyntaxAsm());
                }
                else if (file.Name.EndsWith(".il"))
                {
                    code.Parser = new SourceParser(new SyntaxIL());
                }
                else
                {
                    code.Parser = new SourceParser(new SyntaxCSharp());
                }
                sections.Add(stack.Add(code));
            }

            current               = stack.ChildrenAre<SourceCodeSection>().First();
            current.IsHighlighted = true;



        }

        protected override void InitSceneComplete()
        {
            
        }

        protected override void DrawOverlay(DrawContext drawing)
        {
            
        }

        protected override void DrawBackGround(DrawContext drawing)
        {
            drawing.Canvas.Clear(Styles.GetColor(this, "bg"));
        }

        public override void ProcessEvent(object platform, string name, object args)
        {
            
        }

        public override void KeyPress(object platformKeyObject, string key)
        {
            int i;
            switch (key)
            {
                
                case "s":
                    this.InitSceneComplete();
                    break;
                
                
                case "Key_2":
                    SendHostCommand?.Invoke("Scene", "Execute");
                    break;
                
                case "d":
                case "n":
                case "Right":    
                case "period":
                    i = sections.IndexOf(current);
                    if (i < sections.Count - 1)
                    {
                        current.IsHighlighted = false;
                        current               = sections[i + 1];
                        current.IsHighlighted = true;
                    }
                    break;
                
                case "a":
                case "p":
                case "Left":
                case "comma":
                    i = sections.IndexOf(current);
                    if (i > 0)
                    {
                        current.IsHighlighted = false;
                        current               = sections[i - 1];
                        current.IsHighlighted = true;
                    }
                    break;
                
                case "q":
                    SendHostCommand?.Invoke("QUIT", null);
                    break;
            }
        }

        public override void MousePress(uint eventButton, double eventX, double eventY, object interop)
        {
            
        }
    }
}