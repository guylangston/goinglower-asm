using System.Collections.Generic;
using System.IO;
using GoingLower.Core.Helpers;

namespace GoingLower.CPU.Model
{
    
    public class SourceFile
    {
        public string                Name { get; set; }
        public string                Path  { get; set; }
        
        public string                AltName { get; set; }
        public IReadOnlyList<string> Lines   { get; set; }
        public string?               Title   { get; set; }
        public string                Format  { get; set; }

        public override string ToString() => Name ?? Path;
    }

    public class SourceFileAnchor
    {
        public SourceFile File        { get; set; }
        public uint       Line        { get; set; }
        public int        RegionStart { get; set; } = -1;
        public int        RegionEnd   { get; set; } = -1;
        public string?    LineText    => File?.Lines != null && Line > 0 &&  Line < File.Lines.Count 
            ? File.Lines[(int)Line - 1] : null;

        public override string ToString() => $"L{Line}: {LineText?.Trim()}";
    }
    
    public class SourceProvider
    {
        private Dictionary<string, SourceFile> files;
        private string targetBin;
        
        public SourceProvider(string targetBin)
        {
            this.targetBin = targetBin;
            files          = new Dictionary<string, SourceFile>();
        }

        public string TargetBinary => targetBin;
        
        public IReadOnlyDictionary<string, SourceFile> Files => files;

        public SourceFile Load(string txtFile)
        {
            if (Files.ContainsKey(txtFile)) return Files[txtFile];

            var s = new SourceFile()
            {
                Name   = Path.GetFileName(txtFile),
                Title  = Path.GetFileName(txtFile),
                Format = Path.GetExtension(txtFile).Remove(0, 1),
                Path   = txtFile,
                Lines  = File.ReadAllLines(txtFile)
            };
            files[s.Path] = s;
            return s;
        }
        
        public SourceFileAnchor? FindAnchor(string? currSource)
        {
            if (currSource == null) return null;
            
            // Sample: "home/guy/RiderProjects/ConsoleApp1/ConsoleApp1/Program.cs @ 37:"
            if (StringHelper.TrySplitExclusive(currSource.Trim(':'), " @ ", out var res))
            {
                var f = res.l;
                if (!File.Exists(f))
                {
                    f = TryFindAlternativeRoot(f);
                }

                if (f == null) return null;

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

        protected virtual string? TryFindAlternativeRoot(string s)
        {
            var x = 1;
            return null;
        }
    }
}