using System;
using GoingLower.Core.Elements;

namespace GoingLower.Core.Helpers
{
    public static class TextElementHelper
    {
        public static void WriteHexWords(this TextBlockElement e, ulong val, int size)
        {
            // 64,32,16,8
            // 32 = "XX:XX-XX:XX"
            // 62 = "XX:XX:XX:XX-XX:XX:XX:XX"

            var gray  = e.Scene.StyleFactory.GetPaint(e, "FixedFontGray");
            var sep  = e.Scene.StyleFactory.GetPaint(e, "FixedFontDarkGray");
            var pair = e.Scene.StyleFactory.GetPaint(e, "FixedFontCyan");

            var txt = val.ToString("X").PadLeft(size / 4, '0');
            var cc  = 0;
            while (cc < txt.Length)
            {
                if (cc > 0)
                {
                    if (size == 64 && cc == 8)
                    {
                        e.Write("-", sep);
                    }
                    else if (size == 32 && cc == 4)
                    {
                        e.Write("-", sep);
                    }
                    else
                    {
                        e.Write(":", sep);    
                    }
                    
                }
                var seg = txt.Substring(cc, 2);
                e.Write(seg, pair);
                
                cc += 2;
            }
        }
        
        public static TextBlockElement.Span WriteUrl(this TextBlockElement e, string url, string txt)
        {
            var gray = e.Scene.StyleFactory.GetPaint(e, "FixedFontDarkGray");
            e.Write("[↗", gray);
            var span = e.Write(txt, e.Scene.StyleFactory.GetPaint(e, "url"));
            span.Url = url;
            e.Write("]", gray);
            
            return span;
        }
        
        public static void WriteLineFormatted(this TextBlockElement e, FormattableString fs)
        {
            WriteFormatted(e, fs);
            e.WriteLine();
        }


        public static void WriteFormatted(this TextBlockElement e, FormattableString fs)
        {
            var f = fs.Format;
            if (f.Length < 3)
            {
                e.Write(f);
                return;
            }
            
            var i = 0;
            var s = 0;
            
            while (i < f.Length-1)
            {
                if (f[i] == '{' && char.IsDigit(f[i+1]))
                {
                    var ss = i;
                    i++;
                    while (char.IsDigit(f[i]))
                    {
                        i++;
                    }
                    if (f[i] == '}')
                    {
                        // Segment dont
                        e.Write(f[s..ss]);
                        var idx = int.Parse(f[(ss+1)..i]);
                        e.Write(fs.GetArgument(idx), e.Scene.StyleFactory.GetPaint(e, "arg"));
                        
                    }
                    s = i+1;
                }

                i++;
            }
        }
    }
}