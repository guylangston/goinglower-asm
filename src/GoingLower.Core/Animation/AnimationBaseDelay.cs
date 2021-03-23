using System;

namespace GoingLower.Core.Animation
{
    public class AnimationBaseDelay : AnimationBase
    {
        public AnimationBaseDelay(TimeSpan duration) : base(duration)
        {
            
        }

        
        protected override void StartInner()
        {
            
        }
        
        protected override bool StepInner(TimeSpan t)
        {
            return Elapsed >= Duration;
        }
        
    }
}