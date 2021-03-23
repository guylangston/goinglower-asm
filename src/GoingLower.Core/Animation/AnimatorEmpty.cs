using System;
using System.Collections.Generic;

namespace GoingLower.Core.Animation
{
    public class AnimatorEmpty : IAnimator
    {
        private AnimatorEmpty()
        {
        }

        public static readonly AnimatorEmpty Instance = new AnimatorEmpty();


        public bool IsActive => false;
        public void Start()
        {
            
        }
        public bool Step(TimeSpan t)
        {
            return true;
        }
        
        public void Stop()
        {
            
        }
        public void Add(IAnimation a)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IAnimation> Animations => null;
    }
}