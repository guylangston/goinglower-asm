using System;
using System.Collections.Generic;

namespace GoingLower.Core.Animation
{
    public abstract class AnimatorBase : AnimationBase, IAnimator
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
}