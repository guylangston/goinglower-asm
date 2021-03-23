using System;


namespace GoingLower.Core.Animation
{
    public class AnimationBaseProp : AnimationBase
    {
        public  PropFloat Target   { get; set; }
        public  float     A        { get; set; }
        public  float     B        { get; set; }

        public AnimationBaseProp(PropFloat target, float a, float b, TimeSpan duration) : base(duration)
        {
            Target   = target;
            A        = a;
            B        = b;
        }

        protected override void StartInner()
        {
            
        }
        
        protected override bool StepInner(TimeSpan t)
        {
            if (Elapsed >= Duration)
            {
                Target.Value = B;
                return true;
            }

            var r = Math.Abs((float)Elapsed.TotalSeconds / (float)Duration.TotalSeconds);
            Target.Value = A + (B - A) * r;

            return false;
        }
        
    }

   
}