using System;
using System.Collections.Generic;

namespace GoingLower.Core
{
    public interface IAnimation
    {
        bool IsActive { get;  }
        void Start();
        bool Step(TimeSpan t);
        void Stop();
        
    }

    public interface IAnimator : IAnimation // Does not draw, just
    {
        void Add(IAnimation a);
        IEnumerable<IAnimation> Animations { get; }
    }
    
    // Animatable property
    public interface IAnimProp
    {
        public float Value     { get; set; }
        public float BaseValue { get; set; }
    }

    public class PropFloat : IAnimProp
    {
        public PropFloat(float baseValue)
        {
            Value = BaseValue = baseValue;
        }

        public PropFloat(float value, float baseValue)
        {
            Value     = value;
            BaseValue = baseValue;
        }

        public float Value     { get; set; }
        public float BaseValue { get; set; }
    }

    
    
}