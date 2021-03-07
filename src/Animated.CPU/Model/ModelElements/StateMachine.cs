using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Animated.CPU.Model
{
     
    public record State<T>
    {
        public State(string id, T target, Action<State<T>>? onEnter = null, Action<State<T>>? onLeave = null)
        {
            Id      = id;
            Target  = target;
            OnEnter = onEnter;
            OnLeave = onLeave;
        }

        public string            Id      { get; }
        public T                 Target  { get; }
        public Action<State<T>>? OnEnter { get; }
        public Action<State<T>>? OnLeave { get; }

        protected virtual bool PrintMembers(StringBuilder builder)
        {
            builder.Append($"[{Id}] {Target}");
            return true;
        }
    }

    
    public abstract class StateMachine<T>
    {
        private  readonly List<State<T>> states = new List<State<T>>();
        private State<T> current;
        private readonly bool allowSequential;

        protected StateMachine(bool allowSequential)
        {
            this.allowSequential = allowSequential;
        }

        protected State<T> Add(State<T> state)
        {
            states.Add(state);
            return state;
        }

        public IReadOnlyCollection<State<T>> All => states;

        protected void CompleteInit(State<T> start)
        {
            current = start;
            current.OnEnter?.Invoke(current);
        }
        
        
        public State<T> Current
        {
            get => current;
            set
            {
                if (current == value) return;
                current.OnLeave?.Invoke(current);
                current = value;
                current.OnEnter?.Invoke(current);
            }
        }
        
       
        public abstract void ExecStart();
        public abstract void ExecNext();
        public abstract void ExecPrev();
        
        public int GetSeq(T target)
        {
            if (!allowSequential) throw new Exception();
            for (int i = 0; i < states.Count; i++)
            {
                if (states[i].Target.Equals(target)) return i;
            }
            return -1;

        }

        public int GetSeq(State<T> state)
        {
            if (!allowSequential) throw new Exception();
            return states.IndexOf(state);
        }
        
        protected virtual State<T> NextInSeq()
        {
            if (!allowSequential) throw new Exception();
            
            var i = states.IndexOf(Current);
            if (i < states.Count -1) i++;
            return states[i];
        }
        
        protected virtual State<T> PrevInSeq()
        {
            if (!allowSequential) throw new Exception();
            
            var i = states.IndexOf(Current);
            if (i > 0) i--;
            return states[i];
        }
    }

    
}