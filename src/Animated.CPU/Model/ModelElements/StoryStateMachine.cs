using System;
using System.Collections.Generic;
using System.Linq;
using Animated.CPU.Animation;

namespace Animated.CPU.Model
{
    
    
    public class StoryStateMachine : StateMachine<IElement>
    {
        private readonly SceneExecute scene;
        private readonly LogicUnitElement logicUnit;

        public StoryStateMachine(SceneExecute scene, LogicUnitElement logicUnit) : base(true)
        {
            this.scene = scene;
            this.logicUnit   = logicUnit;
            
            Add(Start       = new State<IElement>(nameof(Start), logicUnit));
            Add(Fetch       = new State<IElement>(nameof(Fetch), logicUnit.Fetch));
            Add(Decode      = new State<IElement>(nameof(Decode), logicUnit.Decode));
            Add(ExecuteInp  = new State<IElement>(nameof(ExecuteInp), logicUnit.ExecuteINP, logicUnit.ExecuteINP.StateChangeOnEnter));
            Add(ExecuteOut  = new State<IElement>(nameof(ExecuteOut), logicUnit.ExecuteOUT, logicUnit.ExecuteOUT.StateChangeOnEnter));
            Add(StepForward = new State<IElement>(nameof(StepForward), logicUnit));
            Add(Finished    = new State<IElement>(nameof(Finished), logicUnit));
            
            
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
            logicUnit.Story.CurrentIndex = 0;
        }

        public override void ExecNext()
        {
            if (Current == StepForward)
            {
                // loop back until we run out of steps
                if (logicUnit.Story.CurrentIndex < logicUnit.Story.Steps.Count-1)
                {
                    logicUnit.Story.CurrentIndex++;
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
                if (logicUnit.Story.CurrentIndex > 0)
                {
                    logicUnit.Story.CurrentIndex--;
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

        public void NextInstruction()
        {
            if (logicUnit.Story.CurrentIndex < logicUnit.Story.Steps.Count-1)
            {
                logicUnit.Story.CurrentIndex++;
                Current = Fetch;
            }
        }

        public void PrevInstruction()
        {
            if (logicUnit.Story.CurrentIndex > 0)
            {
                logicUnit.Story.CurrentIndex--;
                Current = Fetch;
            }
        }
    }
}