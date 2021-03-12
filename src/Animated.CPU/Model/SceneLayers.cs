using System;
using Animated.CPU.Animation;

namespace Animated.CPU.Model
{
    public class SceneLayers : SceneBase<Story, StyleFactory>
    {
        public SceneLayers(StyleFactory styleFactory) : base(styleFactory)
        {
        }
        
        public bool UseEmbelishments { get; set; } = true;

        protected override void InitScene()
        {
            
            if (UseEmbelishments)
            {
                for (int cc = 0; cc < 100; cc++)
                    Add(new BackGroundNoise(this, Block));
            }

            var ww = 1920;
            var hh = 1090;

            float w = Block.Inner.W / 4;

            // var cpu = Add(new SimpleSection(this, "CPU", 
            //     DBlock.FromTwoPoints(new SKPoint(50, 20), new SKPoint(970, 1020))));
            // cpu.TitleAction = "CPU";
            //
            // var ram = Add(new SimpleSection(this, "RAM", 
            //     DBlock.FromTwoPoints(new SKPoint(990, 20), new SKPoint(1440, 1020))));
            // ram.TitleAction = "RAM";
            //

            var stack = Add(new StackElement(this, Block, DOrient.Horz));
            //
            // this.ElementCode = stack.Add(new CodeElement(stack, Model.Story.MainFile, 
            //     DBlock.JustWidth(w)));
            //
            // if (Cpu.Story.IL != null)
            // {
            //     this.ElementCodeIL = stack.Add(new CodeElement(stack, Cpu.Story.IL, 
            //         DBlock.JustWidth(w)));
            //     this.ElementCodeIL.IsHidden = true;
            // }
            //
            // if (Cpu.Story.Asm != null)
            // {
            //     this.ElementCodeASM = stack.Add(new CodeElement(stack, Cpu.Story.Asm, 
            //         DBlock.JustWidth(w)));
            //     this.ElementCodeASM.IsHidden = true;
            // }
        }

        protected override void InitSceneComplete()
        {
            throw new NotImplementedException();
        }

        protected override void DrawOverlay(DrawContext drawing)
        {
            throw new NotImplementedException();
        }

        protected override void DrawBackGround(DrawContext drawing)
        {
            throw new NotImplementedException();
        }

        public override void ProcessEvent(object platform, string name, object args)
        {
            throw new NotImplementedException();
        }

        public override void KeyPress(object platformKeyObject, string key)
        {
            throw new NotImplementedException();
        }

        public override void MousePress(uint eventButton, double eventX, double eventY, object interop)
        {
            throw new NotImplementedException();
        }
    }
}