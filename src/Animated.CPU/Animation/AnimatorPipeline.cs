using System;
using System.Collections.Generic;
using System.Linq;

namespace Animated.CPU.Animation
{
    
   

    public abstract class AnimatorBase : Animation, IAnimator
    {
        protected List<IAnimation> Items { get; } = new List<IAnimation>();
        
        protected AnimatorBase(TimeSpan duration) : base(duration)
        {
        }
        
        public IEnumerable<IAnimation> Animations => Items;
        
        public void Add(IAnimation a)
        {
            Items.Add(a);
        }

        public override void Stop()
        {
            foreach (var animation in Animations)
            {
                animation.Stop();
            }
            base.Stop();
        }
    }
    
    public class AnimatorPipeline : AnimatorBase
    {
        public bool Loop { get; set; }
        
        public AnimatorPipeline(TimeSpan duration) : base(duration)
        {
        }
        
        protected override bool StepInner(TimeSpan t)
        {
            bool readyNext = false;
            foreach (var animation in Items)
            {
                if (readyNext)
                {
                    animation.Start();
                    readyNext = false;
                }
                
                if (animation.IsActive)
                {
                    if (animation.Step(t))
                    {
                        animation.Stop();
                        readyNext          = true;
                    }    
                }
            }

            var complete = Items.All(x => !x.IsActive);

            if (Loop && complete)
            {
                StartInner();
                return false;
            }
            else
            {
                return complete;    
            }
        }

        protected override void StartInner()
        {
            if (Items.Any())
            {
                Items[0].Start();
            }
        }
    }
}