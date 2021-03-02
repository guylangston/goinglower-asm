using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using Animated.CPU.Animation;
using SkiaSharp;

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

        public override string ToString() => File.Lines[(int)Line].Trim();
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
            public SourceFileAnchor? Anchor    { get; set; }

            public override string ToString() => 
                $"[{Address:X}|{Offset}] {RawAsString(),16}, {Label} {SourceAsm} {Comment} // {Anchor}";

            public string RawAsString() => String.Join("", Raw.Select(x => x.ToString("X")));
            
        }


        public Segment? GetByAddress(ulong addr) 
            => Segments.FirstOrDefault(x => x.Address <= addr && addr < x.Address + (ulong)x.Raw.Length);
    }
    
    public class MemoryViewElement : Element<Scene, MemoryView>
    {

        public MemoryViewElement(Scene scene, DBlock b, MemoryView memory) : base(scene, memory, b)
        {
            
        }
       
        
        public override void Init(DrawContext surface)
        {
            var stack = new StackElement(Scene, this, Block, DOrient.Vert);
            Add(stack);

            foreach (var seg in Model.Segments)
            {
                stack.Add(new SegmentElement(stack, seg, new DBlock()
                {
                    H = 30
                }));
            }
        }

        
        protected override void Step(TimeSpan step)
        {
            
        }
        
        protected override void Draw(DrawContext surface)
        {
            
        }
    }

    public class SegmentElement : Element<Scene, MemoryView.Segment>
    {

        public SegmentElement(Scene scene, MemoryView.Segment model) : base(scene, model)
        {
        }
        public SegmentElement(IElement parent, MemoryView.Segment model) : base(parent, model)
        {
        }
        public SegmentElement(Scene scene, MemoryView.Segment model, DBlock block) : base(scene, model, block)
        {
        }
        public SegmentElement(IElement parent, MemoryView.Segment model, DBlock block) : base(parent, model, block)
        {
        }

        public override void Init(DrawContext surface)
        {
            Add(new ByteArrayElement(this, 
                new ByteArrayModel(Model.Raw, Model.SourceAsm, Model.Comment), 
                new DBlock(Block.Inner.X + 1, Block.Inner.Y + 5, Block.Inner.W - 20, Block.Inner.H-20)));
        }

        protected override void Step(TimeSpan step)
        {
            
        }

        protected override void Draw(DrawContext surface)
        {
            var canvas = surface.Canvas;
            var draw   = new Drawing(canvas);
            
            var sBorder = Scene.StyleFactory.GetPaint(this, "border");
            var sText   = Scene.StyleFactory.GetPaint(this, "text");
            draw.DrawRect(Block, sBorder);
            
            //draw.DrawTextCenter(Model?.ToString(), sText, Block.Inner.MM);
            draw.DrawText($"+{Model.Offset}", sText, Block, BlockAnchor.BR);

            draw.DrawText(""+Model.Comment, sText, Block, BlockAnchor.MR);
            
            draw.DrawText(""+Model.Label, sText, Block, BlockAnchor.TR);
        }
    }

}