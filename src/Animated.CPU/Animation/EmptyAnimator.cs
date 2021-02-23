using System;
using System.Collections.Generic;

namespace Animated.CPU.Animation
{
    public class EmptyAnimator : IAnimator
    {
        private EmptyAnimator()
        {
        }

        public static readonly EmptyAnimator Instance = new EmptyAnimator();


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