using System;
using System.Collections.Generic;
using System.Linq;
using Animated.CPU.Animation;

namespace Animated.CPU.Model
{
    
    
    public class StoryStateMachine : StateMachine<IElement>
    {
        private readonly Scene scene;
        private readonly ALUElement alu;

        public StoryStateMachine(Scene scene, ALUElement alu) : base(true)
        {
            this.scene = scene;
            this.alu   = alu;
            
            Add(Start       = new State<IElement>(nameof(Start), alu));
            Add(Fetch       = new State<IElement>(nameof(Fetch), alu.Fetch));
            Add(Decode      = new State<IElement>(nameof(Decode), alu.Decode));
            Add(ExecuteInp  = new State<IElement>(nameof(ExecuteInp), alu.ExecuteINP, alu.ExecuteINP.StateChangeOnEnter));
            Add(ExecuteOut  = new State<IElement>(nameof(ExecuteOut), alu.ExecuteOUT, alu.ExecuteOUT.StateChangeOnEnter));
            Add(StepForward = new State<IElement>(nameof(StepForward), alu));
            Add(Finished    = new State<IElement>(nameof(Finished), alu));
            
            
            CompleteInit(Start);
        }

        public State<IElement> Start       { get; }
        public State<IElement> Fetch       { get; }
        public State<IElement> Decode      { get; }
        public State<IElement> ExecuteInp  { get; }
        public State<IElement> ExecuteOut  { get; }
        public State<IElement> StepForward { get; }
        public State<IElement> Finished    { get; }
        
        
        

        public override void ExecStart()
        {
            Current                = Start;
            alu.Story.CurrentIndex = 0;
        }

        public override void ExecNext()
        {
            if (Current == StepForward)
            {
                // loop back until we run out of steps
                if (alu.Story.CurrentIndex < alu.Story.Steps.Count-1)
                {
                    alu.Story.CurrentIndex++;
                    Current = Fetch;
                    return;
                }
            }
            
            Current = NextInSeq();

        }

        
        public override void ExecPrev()
        {
            if (Current == Fetch)
            {
                if (alu.Story.CurrentIndex > 0)
                {
                    alu.Story.CurrentIndex--;
                    Current = ExecuteOut;
                    return;
                }
            }
            else if (Current == Finished)
            {
                Current = ExecuteOut;
                return;
            }
            
            Current = PrevInSeq();
        }
    }
}