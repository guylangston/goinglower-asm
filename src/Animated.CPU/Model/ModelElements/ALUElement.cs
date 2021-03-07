using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using Animated.CPU.Animation;

namespace Animated.CPU.Model
{
    
    
    
    public class ALUElement : Section<Scene, ArithmeticLogicUnit>
    {
        public ALUElement(IElement scene, ArithmeticLogicUnit alu, DBlock b) : base(scene, alu, b)
        {
            Title = "ALU";
        }

        protected override void Init()
        {
            var stack = Add(new StackElement(this, Block, DOrient.Vert));
            this.Fetch      = stack.Add(new FetchPhaseElement(stack, Model.Fetch,this));
            this.Decode     = stack.Add(new DecodePhaseElement(stack, Model.Decode,this));
            this.ExecuteINP = stack.Add(new ExecutePhaseElementINP(stack, Model.Execute,this));
            this.ExecuteOUT = stack.Add(new ExecutePhaseElementOUT(stack, Model.Execute,this));

            StateMachine = new StoryStateMachine(Scene, this);
        }

        public FetchPhaseElement      Fetch        { get; private set; }
        public DecodePhaseElement     Decode       { get; private set; }
        public ExecutePhaseElementINP ExecuteINP   { get; private set; }
        public ExecutePhaseElementOUT ExecuteOUT   { get; private set; }
        public StoryStateMachine      StateMachine { get; private set; }
        public IElement               Active       => StateMachine.Current.Target;
        public Story                  Story        => Scene.Model.Story;
        
        

        protected override void Step(TimeSpan step)
        {
            if (StateMachine.Current == StateMachine.StepForward) StateMachine.ExecNext(); 
        }

        public void Start()
        {
            if (!InitComplete) throw new Exception("Not Init");
            StateMachine.ExecStart();
        }
        

        public void Next()
        {
            if (!InitComplete) throw new Exception("Not Init");
            StateMachine.ExecNext();
        }

        public void Prev()
        {
            if (!InitComplete) throw new Exception("Not Init");

            StateMachine.ExecPrev();
        }
    }
}