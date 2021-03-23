using System;
using System.Linq;

namespace GoingLower.Core.Animation
{
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