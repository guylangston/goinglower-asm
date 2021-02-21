using System;

namespace Animated.CPU.Animation
{
    public class CountDown
    {
        public CountDown(float duration)
        {
            Remaining = Duration = duration;
        }

        public float Duration  { get; set; }
        public float Remaining { get; set; }

        public bool Step(TimeSpan s)
        {
            Remaining -= (float)s.TotalSeconds;
            if (Remaining <= 0)
            {
                Remaining = Duration;
                return true;
            }

            return false;
        }
    }
}