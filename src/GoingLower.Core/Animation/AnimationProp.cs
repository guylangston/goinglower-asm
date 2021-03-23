using System;


namespace GoingLower.Core.Animation
{
    public abstract class Animation : IAnimation
    {
        protected abstract void StartInner();
        protected abstract bool StepInner(TimeSpan t);

        protected Animation(TimeSpan duration)
        {
            Duration = duration;
        }

        public TimeSpan Duration { get; private set; }
        public TimeSpan Elapsed  { get; private set; }
        public  bool     IsActive { get; private set; }
        
        public void Start()
        {
            IsActive = true;
            Elapsed = TimeSpan.Zero;
            StartInner();
        }
        
        public bool Step(TimeSpan t)
        {
            Elapsed += t;
            var complete = StepInner(t) || Elapsed >= Duration;
            if (complete ) Stop();
            return complete;
        }
        
        public virtual void Stop()
        {
            IsActive = false;
        }
    }
    
    public class AnimationProp : Animation
    {
        public  PropFloat Target   { get; set; }
        public  float     A        { get; set; }
        public  float     B        { get; set; }

        public AnimationProp(PropFloat target, float a, float b, TimeSpan duration) : base(duration)
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