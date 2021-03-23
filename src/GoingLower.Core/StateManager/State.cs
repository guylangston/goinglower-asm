using System;
using System.Text;

namespace GoingLower.Core.StateManager
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
}