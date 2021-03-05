using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using Animated.CPU.Animation;
using SkiaSharp;

namespace Animated.CPU.Model
{
    public enum EState
    {
        Start,
        Fetch,
        Decode,
        ExecuteInp,
        ExecuteOut,
        StepForward,
        Finished
    }
    
    public class StateMachine<T>
    {
        private List<State>

        private Scene Scene { get; }

        public EState   Current        { get; private set; }
        public IElement CurrentElement => map[Current];
        
        public class State
        {
            public string        Id      { get; set; }
            public T             Target  { get; set; }
            public Action<State> OnEnter { get; set; }
            public Action<State> OnLeave { get; set; }
        }
        
        
        public void Start()
        {
            Current = EState.Start;
        }

        public void Next()
        {
            if (Current == EState.StepForward)
            {
                if (Scene.Cpu.Story.CurrentIndex < Scene.Cpu.Story.Steps.Count)
                {
                    Scene.Cpu.Story.CurrentIndex++;
                    Current = EState.Fetch;
                    return;    
                }
                else
                {
                    Current = EState.Finished;
                    return;
                }
            }
            
            var c = (int)Current;
            Current = (EState)(c + 1);

        }

        public void Prev()
        {
            if (Current == EState.Fetch)
            {
                if (Scene.Cpu.Story.CurrentIndex > 0)
                {
                    Scene.Cpu.Story.CurrentIndex--;
                    Current = EState.ExecuteOut;
                    return;    
                }
                else
                {
                    Current = EState.Start;
                    return;
                }
            }
            
            var c = (int)Current;
            Current = (EState)(c - 1);
        }
    }


    

    public class StateMachine
    {
        private Dictionary<EState, IElement> map = new Dictionary<EState, IElement>();

        private Scene Scene { get; }

        public EState   Current        { get; private set; }
        public IElement CurrentElement => map[Current];
        
        public class State
        {
            public EState        Id      { get; set; }
            public IElement      Target  { get; set; }
            public Action<State> OnEnter { get; set; }
            public Action<State> OnLeave { get; set; }
        }
        
        
        public void Start()
        {
            Current = EState.Start;
        }

        public void Next()
        {
            if (Current == EState.StepForward)
            {
                if (Scene.Cpu.Story.CurrentIndex < Scene.Cpu.Story.Steps.Count)
                {
                    Scene.Cpu.Story.CurrentIndex++;
                    Current = EState.Fetch;
                    return;    
                }
                else
                {
                    Current = EState.Finished;
                    return;
                }
            }
            
            var c    = (int)Current;
            Current = (EState)(c + 1);

        }

        public void Prev()
        {
            if (Current == EState.Fetch)
            {
                if (Scene.Cpu.Story.CurrentIndex > 0)
                {
                    Scene.Cpu.Story.CurrentIndex--;
                    Current = EState.ExecuteOut;
                    return;    
                }
                else
                {
                    Current = EState.Start;
                    return;
                }
            }
            
            var c = (int)Current;
            Current = (EState)(c - 1);
        }
    }
    
    
    public class ALUElement : Section<Scene, ArithmeticLogicUnit>
    {
        public ALUElement(IElement scene, ArithmeticLogicUnit alu, DBlock b) : base(scene, alu, b)
        {
            Title = "ALU";
        }

        protected override void Init()
        {
            var stack = Add(new StackElement(this, Block, DOrient.Vert));
            this.Fetch   = stack.Add(new FetchPhaseElement(stack, Model.Fetch,this));
            this.Decode  = stack.Add(new DecodePhaseElement(stack, Model.Decode,this));
            this.Execute = stack.Add(new ExecutePhaseElement(stack, Model.Execute,this));
        }

        public FetchPhaseElement   Fetch   { get; set; }
        public DecodePhaseElement  Decode  { get; set; }
        public ExecutePhaseElement Execute { get; set; }

        public Story Story => Scene.Model.Story;
        
        public IElement Active { get; set; }


        public void Start()
        {
            if (!InitComplete) throw new Exception("Not Init");
            Active             = Fetch;
            Story.CurrentIndex = 0;
        }
        

        public void Next()
        {
            if (!InitComplete) throw new Exception("Not Init");
            Active.Next();
        }

        public void Prev()
        {
            if (!InitComplete) throw new Exception("Not Init");

            Active.Prev();
        }
    }
    

    public class FetchPhaseElement : Section<Scene, PhaseFetch>
    {
        private TextBlockElement text;
        ALUElement master;

        public FetchPhaseElement(IElement parent, PhaseFetch model, ALUElement master) : base(parent, model, new DBlock()
        {
            H = 100
        })
        {
            this.master = master;
            Title       = "Fetch";
            Block.Set(4, 1, 10);
            Block.Margin = new DBorder(30, 4, 4, 4);
        }
        
        public void Next()
        {
            master.Active = master.Decode;
        }

        public void Prev()
        {
            if (master.Story.CurrentIndex > 0)
            {
                master.Story.CurrentIndex--;
                master.Active = master.Execute;
            }
        }

        
        protected override void Init()
        {
            text = Add(new TextBlockElement(this, Block, Scene.Styles.FixedFont));
        }

        protected override void Step(TimeSpan step)
        {
            if (Model.Memory != null)
            {
                text.Clear();

                text.Write("RIP".PadRight(12));
                text.Write(": ");
                text.WriteLine(DisplayHelper.ToHex(Model.RIP), Scene.Styles.FixedFontCyan);

                text.Write("Binary Code".PadRight(12));
                text.Write(": ");
                text.WriteLine(DisplayHelper.ToHex(Model.Memory), Scene.Styles.FixedFontBlue);
            }
            //Block.H = Math.Max(60, text.LastDrawHeight);
        }

      

        protected override void Decorate(DrawContext surface)
        {
            if (master.Active == this)
            {
                surface.DrawHighlight(this);
                
                var seg = Scene.Cpu.Instructions.GetByAddress(Model.RIP);
                if (seg != null && Scene.TryRecurseElementFromModel(seg, out var eRip))
                {
                    var a = Block.Outer.MR;
                    var b = eRip.Block.Inner.ML;
                    new Arrow()
                    {
                        Start     = a,
                        WayPointA = a + new SKPoint(20, 0),
                        WayPointB = b + new SKPoint(-20, 0),
                        End       = b,
                        Style     = Scene.Styles.Arrow
                    }.Draw(surface.Canvas);
                }    
            }
        }

        
    }

    public class DecodePhaseElement : Section<Scene, PhaseDecode>, IHasMaster<ALUElement>
    {
        private TextBlockElement text;
        ALUElement master;

        public DecodePhaseElement(IElement parent, PhaseDecode model, ALUElement master) : base(parent, model, new DBlock()
        {
            H = 120
        })
        {
            this.master = master;
            Block.Set(4, 1, 10);
            Block.Margin = new DBorder(30, 4, 4, 4);
            Title        = "Decode";
        }
        

        protected override void Init()
        {
            text = Add(new TextBlockElement(this, Block, Scene.Styles.FixedFont));
        }

        protected override void Step(TimeSpan step)
        {
            text.Clear();
            var decode = Model.Asm;
            if (decode != null)
            {
                text.WriteLine(decode);
                text.WriteLine(decode.FriendlyName, Scene.Styles.FixedFontBlue);
                text.WriteLine(decode.FriendlyMethod, Scene.Styles.FixedFontBlue);
            }
        }
        
        protected override void Decorate(DrawContext surface)
        {
            if (master.Active == this)
            {
                surface.DrawHighlight(this);
                
                var seg = Scene.Cpu.Instructions.GetByAddress(master.Story.Current.RIP);
                if (seg != null && Scene.TryRecurseElementFromModel(seg, out var eRip))
                {
                    var a = Block.Outer.MR;
                    var b = eRip.Block.Inner.ML;
                    new Arrow()
                    {
                        Start     = a,
                        WayPointA = a + new SKPoint(20, 0),
                        WayPointB = b + new SKPoint(-20, 0),
                        End       = b,
                        Style     = Scene.Styles.Arrow
                    }.Draw(surface.Canvas);
                }    
            }
        }
    }

    public class ExecutePhaseElement : Section<Scene, PhaseExecute>, IHasMaster<ALUElement>
    {
        private TextBlockElement text;
        ALUElement master;

        public ExecutePhaseElement(IElement parent, PhaseExecute model, ALUElement master) : base(parent, model, new DBlock()
        {
            H = 300
        })
        {
            this.master = master;
            Title       = "Execute";
            Block.Set(4, 1, 10);
            Block.Margin = new DBorder(30, 4, 4, 4);
        }
        

        protected override void Init()
        {
            text = Add(new TextBlockElement(this, Block, Scene.Styles.FixedFont));
        }

        protected override void Step(TimeSpan step)
        {
            text.Block = this.Block;
            text.Clear();

            var decode = Model.Asm;
            if (decode?.Args != null && decode.Args.Any())
            {
                foreach (var arg in decode.Args.Distinct(DecodedArg.Compare))
                {
                    var val = Model.Alu.GetInput(decode, arg);
                    text.Write($" IN A{arg.Index}: ");
                    text.Write(arg.Value, Scene.Styles.FixedFontCyan);
                    text.Write(" ");
                    if (val != null)
                    {
                        text.Write(val.ValueRaw, Scene.Styles.FixedFontYellow);
                        text.Write("=");
                        text.WriteLine(val.ValueParsed, Scene.Styles.FixedFontCyan);
                    }
                    else
                    {
                        text.WriteLine("?", Scene.Styles.FixedFontDarkGray);
                    }
                }

                text.WriteLine();
                text.Write("---[ ", Scene.Styles.FixedFontDarkGray);
                text.Write(decode.OpCode.ToUpperInvariant(), Scene.Styles.FixedFontYellow);
                text.Write(" ]---", Scene.Styles.FixedFontDarkGray);
                text.WriteLine();
                text.WriteLine(Model.AsmText, Scene.Styles.FixedFontGray);
                text.WriteLine();
                
                foreach (var arg in decode.Args.Distinct(DecodedArg.Compare))
                {
                    var val = Model.Alu.GetOutput(decode, arg);

                    text.Write($"OUT A{arg.Index}: ");
                    text.Write(arg.Value.PadRight(10), Scene.Styles.FixedFontCyan);
                    text.Write(" ");
                    if (val != null)
                    {
                        text.Write(val.ValueRaw, Scene.Styles.FixedFontYellow);
                        text.Write("=");
                        text.WriteLine(val.ValueParsed, Scene.Styles.FixedFontCyan);
                    }
                    else
                    {
                        text.WriteLine("?", Scene.Styles.FixedFontDarkGray);
                    }
                }
            }
        }
        
        protected override void Decorate(DrawContext surface)
        {
            if (master.Active == this)
            {
                surface.DrawHighlight(this);
                
                var seg = Scene.Cpu.Instructions.GetByAddress(master.Story.Current.RIP);
                if (seg != null && Scene.TryRecurseElementFromModel(seg, out var eRip))
                {
                    var a = Block.Outer.MR;
                    var b = eRip.Block.Inner.ML;
                    new Arrow()
                    {
                        Start     = a,
                        WayPointA = a + new SKPoint(20, 0),
                        WayPointB = b + new SKPoint(-20, 0),
                        End       = b,
                        Style     = Scene.Styles.Arrow
                    }.Draw(surface.Canvas);
                }    
            }
        }
    }
}