using System;

namespace GoingLower.Core.Animation
{
    public class AnimationDelay : Animation
    {
        public AnimationDelay(TimeSpan duration) : base(duration)
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