using System.Collections.Generic;
using System.IO;

namespace Animated.CPU.Model
{
    
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
        public string?    LineText    => File?.Lines != null && Line > 0 &&  Line < File.Lines.Count 
            ? File.Lines[(int)Line - 1] : null;

        public override string ToString() => $"L{Line}: {LineText?.Trim()}";
    }
    
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
                FileName  = txtFile,
                Lines     = File.ReadAllLines(txtFile)
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
}