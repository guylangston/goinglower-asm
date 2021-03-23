using System;

namespace GoingLower.Core.Animation
{
    public abstract class AnimationBase : IAnimation
    {
        protected abstract void StartInner();
        protected abstract bool StepInner(TimeSpan t);

        protected AnimationBase(TimeSpan duration)
        {
            Duration = duration;
        }

        public TimeSpan Duration { get; private set; }
        public TimeSpan Elapsed  { get; private set; }
        public bool     IsActive { get; private set; }
        
        public void Start()
        {
            IsActive = true;
            Elapsed  = TimeSpan.Zero;
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
}