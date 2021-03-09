using Animated.CPU.Animation;

namespace Animated.CPU.Model
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
                        e.Write("::", gray);
                    }
                    else if (size == 32 && cc == 4)
                    {
                        e.Write("::", gray);
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
    }
}