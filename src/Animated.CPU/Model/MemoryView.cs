using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Animated.CPU.Animation;

namespace Animated.CPU.Model
{
    public class SourceProvider
    {
        private Dictionary<string, SourceFile> files;
        
        public SourceProvider()
        {
            files = new Dictionary<string, SourceFile>();
            
        }
        
        public string                                  TargetBinary { get; set; }
        public IReadOnlyDictionary<string, SourceFile> Files        => files;
        
        
        public SourceFile Load(string txtFile)
        {
            if (Files.ContainsKey(txtFile)) return Files[txtFile];
            
            var s = new SourceFile()
            {
                ShortName = Path.GetFileName(txtFile),
                FileName = txtFile,
                Lines    = File.ReadAllLines(txtFile)
            };
            files[s.FileName] = s;
            return s;
        }
        
        public SourceFileAnchor? FindAnchor(string currSource)
        {
            if (currSource == null) return null;
            // "home/guy/RiderProjects/ConsoleApp1/ConsoleApp1/Program.cs @ 37:"
            
            if (StringHelper.TrySplitExclusive(currSource.Trim(':'), " @ ", out var res))
            {
                var file = Load(res.l);
                if (uint.TryParse(res.r, out var lineNo) && lineNo < file.Lines.Count)
                {
                    return new SourceFileAnchor()
                    {
                        File = file,
                        Line = lineNo,
                    };
                }
            }

            return null;

        }
    }
    
    public class SourceFile
    {
        public string                ShortName { get; set; }
        public string                FileName  { get; set; }
        public IReadOnlyList<string> Lines     { get; set; }

        public override string ToString() => ShortName ?? FileName;
    }

    public class SourceFileAnchor
    {
        public SourceFile File        { get; set; }
        public uint       Line        { get; set; }
        public int        RegionStart { get; set; } = -1;
        public int        RegionEnd   { get; set; } = -1;

        public override string ToString() => $"L{Line}: {File.Lines[(int)Line].Trim()}";
    }
    
    public enum MemoryHint
    {
        Decimal,
        Hex,
        String
    }
    
    public class MemoryView
    {
        public MemoryView(IEnumerable<Segment> setup)
        {
            Offset = 0;
            uint cc = 0;
            
            foreach (var item in setup)
            {
                item.Offset = cc;
                Segments.Add(item);

                cc += (uint)item.Raw.Length;
            }

            Length = cc;
        }

        public uint Offset { get; }
        public uint Length { get; }

        public List<Segment> Segments { get; } = new List<Segment>();
        

        public class Segment
        {
            public uint   Offset { get; set; }
            public byte[] Raw    { get; set; }
             
            public string            Header    { get; set; }
            public string            Label     { get; set; }
            public string            SourceAsm { get; set; }
            public string            Comment   { get; set; }
            public ulong             Address   { get; set; }
            public SourceFileAnchor? SourceAnchor    { get; set; }

            public override string ToString() => 
                $"[{Address:X}|{Offset}] {RawAsString(),16}, {Label} {SourceAsm} {Comment} // {SourceAnchor}";

            public string RawAsString() => String.Join("", Raw.Select(x => x.ToString("X")));
            
        }


        public Segment? GetByAddress(ulong addr) 
            => Segments.FirstOrDefault(x => x.Address <= addr && addr < x.Address + (ulong)x.Raw.Length);
    }
    
    public class MemoryViewElement : Section<Scene, MemoryView>
    {

        public MemoryViewElement(IElement scene, DBlock b, MemoryView memory) : base(scene, memory, b)
        {
            
        }
       
        
        public override void Init(DrawContext surface)
        {
            var stack = new StackElement(this, Block, DOrient.Vert);
            Add(stack);
            
            foreach (var seg in Model.Segments)
            {
                stack.Add(new SegmentElement(stack, seg, new DBlock()));
            }
        }

      
    }

    public class SegmentElement : Element<Scene, MemoryView.Segment>
    {
        private ByteArrayElement mem;
        private TextBlockElement txt;

        public SegmentElement(IElement parent, MemoryView.Segment model, DBlock block) : base(parent, model, block)
        {
            block.Set(0,0,0);
        }

        public override void Init(DrawContext surface)
        {
            txt = Add(new TextBlockElement(this, Block, Scene.Styles.FixedFont));
            // mem = Add(new ByteArrayElement(this, 
            //     new ByteArrayModel(Model.Raw, Model.SourceAsm, Model.Comment), Block));
        }

        protected override void Step(TimeSpan step)
        {
            txt.Block.H = Block.H = (Model.SourceAnchor != null ? txt.LineHeight*4 : txt.LineHeight*3) + 10;
            
            txt.Background = this.IndexInParent % 2 == 0
                ? Scene.Styles.BackGround
                : Scene.Styles.BackGroundAlt;
            

            txt.IsEnabled = Block.Y <= Parent.Block.Inner.Y2;
        }

        protected override void Draw(DrawContext surface)
        {
            txt.Clear();
            txt.Write($"[{DisplayHelper.ToHex(Model.Address)}]", Scene.Styles.FixedFontBlue);
            // if (Model.Offset == 0)
            // {
            //     txt.Write($"[{DisplayHelper.ToHex(Model.Address)}]", Scene.Styles.FixedFontBlue);
            // }
            // else
            // {
            //     txt.Write($"[+{Model.Offset}]", Scene.Styles.FixedFontBlue);
            // }
            txt.Write(" -> ");
            txt.WriteLine(Model.Raw?.ToHex().PadRight(8), Scene.Styles.FixedFontCyan);
            
            if (Model.SourceAnchor != null)
            {
                txt.WriteLine(Model.SourceAnchor, Scene.Styles.FixedFontYellow);
            }
            
            txt.WriteLine(Model.SourceAsm, Scene.Styles.FixedFontGray);


        }
    }

}